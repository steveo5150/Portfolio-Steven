#define _USE_MATH_DEFINES
#include "mex.h"
#include <cstdio>
#include <cstring>
#include <cmath>

// Written by Steven G. Lewis, 2025
// University of Maryland - College Park
// Gemstone Team RUBIX

//================================================================================
//This MEX file reads a WRL 1.0 (VRML-style) geometry file, applies a
//rotation to the stored 3D vertex coordinates, and writes multiple rotated copies
//to disk. The rotation is performed about the Z-axis using a standard 2D rotation
//matrix applied to the (x,y) components while preserving z.
//
//High-level flow - 
//1. Validate MATLAB inputs and open the source file.
//2. Iterate through the requested total rotation in fixed angle steps.
//3. Re-read the file for each step, rotating every vertex encountered.
//4. Output indexed WRL files representing the object at successive angles.
//
//Mathematical operations - 
//For each vertex (x,y,z), compute:
//x' = x*cos(theta) - y*sin(theta)
//y' = x*sin(theta) + y*cos(theta)
//z' = z
//================================================================================

//Forward declaration for helper that copies file contents while rotating vertices
void printfiles(FILE* fin, FILE* fout, double degreesDelta);

//takes four inputs, inputfile, outputfile, degree change per file, total angle to rotate
void mexFunction(int nlhs, mxArray* plhs[], int nrhs, const mxArray* prhs[])
{
    //=========Validate Arg Count=========
    //Expect exactly four inputs from MATLAB.
    if (nrhs != 4) {
        printf("Usage: rotate_wrl_mex(inFile, outFile(no.wrlextension), angleDegDelta, totalAngleDeg)");
    }

    //========Extract MATLAB Inputs Into Native C Buffers========
    char inFile[512];
    char outFile[512];

    //Copy strings from MATLAB mxArrays into C-style strings
    mxGetString(prhs[0], inFile, sizeof(inFile));
    mxGetString(prhs[1], outFile, sizeof(outFile));

    //Scalar rotation parameters
    double angleDegDelta = mxGetScalar(prhs[2]);
    double totalAngleDeg = mxGetScalar(prhs[3]);

    //======Basic Parameter Sanity Checks======
    if (angleDegDelta > totalAngleDeg) {
        mexErrMsgTxt("Error, angle delta > total rotation");
    }
    else if (angleDegDelta <= 0 || totalAngleDeg <= 0) {
        mexErrMsgTxt("Error, angle delta or total rotation cannot be negative");
    }

    //Open source WRL file for reading
    FILE* fin = fopen(inFile, "r");
    if (!fin) {
        mexErrMsgTxt("Error cannot open input file");
    }

    //Compute how many rotated files we will generate
    int totalIter = totalAngleDeg / angleDegDelta;

    //Determine padding width so filenames sort nicely (e.g., _01, _02, ...)
    int digits = (int)ceil(log10((double)totalIter));

    //========Main Loop========
    //Each iteration rewinds the file and produces a new rotated copy.
    for (int i = 0; i < totalIter; i++) {

        //Construct indexed output filename
        char outFileIndexed[512];
        snprintf(outFileIndexed, sizeof(outFileIndexed), "%s_%0*d.wrl", outFile, digits, i);

        //Open destination file
        FILE* fout = fopen(outFileIndexed, "w");
        if (!fout) {
            fclose(fin);
            mexErrMsgTxt("Error cannot open output file");
        }

        //Current rotation angle in degrees
        double rotationAngle = i * angleDegDelta;

        //======Reset file pointer======
        //We reread the original geometry each iteration instead of storing it.
        fseek(fin, 0, SEEK_SET);

        //Delegate actual parsing + rotation
        printfiles(fin, fout, rotationAngle);

        fclose(fout);
    }

    fclose(fin);
    return;
}

//main should only handle args and file openin

void printfiles(FILE* fin, FILE* fout, double degreesDelta) {

    //========Convert degrees to radians========
    //Trig functions operate in radians.
    double theta = degreesDelta * M_PI / 180.0;

    //Precompute cos/sin once for efficiency
    double c = cos(theta);
    double s = sin(theta);

    char buf[256];

    //Tracks whether we are currently inside the "point" block of the WRL file
    bool inPoints = false;

    //========Stream file line-by-line========
    while (fgets(buf, sizeof(buf), fin)) {

        //=====If inside vertex list, rotate coordinates=====
        if (inPoints) {

            //End of point array detected
            if (strchr(buf, ']')) {
                inPoints = false;
                fprintf(fout, "]\n");
            }
            else {

                //Temporary storage for parsed vertex
                double x;
                double y;
                double z;

                int offset;
                char* ptr = buf;

                //Multiple vertices can exist on one line, so iterate manually
                while (true) {

                    //Skip leading whitespace for robust parsing
                    while (*ptr == ' ' || *ptr == '\t') ptr++;

                    int consumed = 0;

                    //Attempt to read a triple (x y z)
                    if (sscanf(ptr, "%lf %lf %lf%n", &x, &y, &z, &consumed) != 3)
                        break;

                    //===========Apply rotation===========
                    //Standard Z-axis rotation matrix
                    double x2 = x * c - y * s;
                    double y2 = x * s + y * c;

                    //Write rotated vertex, preserve original z
                    fprintf(fout, " %.6f %.6f %.6f,", x2, y2, z);

                    //Advance pointer past parsed numbers
                    ptr += consumed;

                    //Skip trailing whitespace
                    while (*ptr == ' ' || *ptr == '\t') ptr++;

                    //Skip optional comma separator
                    if (*ptr == ',')
                        ptr++;
                }

                fprintf(fout, "\n");
            }
        }

        //=======Detect start of vertex block=======
        //Once "point" is found, subsequent lines contain geometry.
        else if (strstr(buf, "point")) {
            fputs(buf, fout);
            inPoints = true;
        }

        //=====Copy all other file content unchanged=====
        //Preserves normals, faces, metadata, etc.
        else {
            fputs(buf, fout);
        }
    }
}