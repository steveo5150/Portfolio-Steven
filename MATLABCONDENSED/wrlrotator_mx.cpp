// WRLRoatator_mx.cpp : This file includes mex function, compile into a mexa or mexw file to use
//
#define _USE_MATH_DEFINES
#include "mex.h"
#include <cstdio>
#include <cstring>
#include <cmath>


void printfiles(FILE* fin, FILE* fout, double degreesDelta);

//takes four args, input, output, degree change per file, total angle to rotate
void mexFunction(int nlhs, mxArray* plhs[], int nrhs, const mxArray* prhs[])
{
    //ensure arguments are valid
    if (nrhs != 4) {
        printf("Usage: rotate_wrl_mex(inFile, outFile(no.wrlextension), angleDegDelta, totalAngleDeg)");
    }
    char inFile[512];
    char outFile[512];
    mxGetString(prhs[0], inFile, sizeof(inFile));
    mxGetString(prhs[1], outFile, sizeof(outFile));
    double angleDegDelta = mxGetScalar(prhs[2]);
    double totalAngleDeg = mxGetScalar(prhs[3]);
    if (angleDegDelta > totalAngleDeg) {
        mexErrMsgTxt("Error, angle delta > total rotation");
    }
    else if (angleDegDelta <= 0 || totalAngleDeg <= 0) {
        mexErrMsgTxt("Error, angle delta or total rotation cannot be negative");
    }
    FILE* fin = fopen(inFile, "r");
    if (!fin) {
        mexErrMsgTxt("Error cannot open input file");
    }

    int totalIter = totalAngleDeg / angleDegDelta;
    int digits = (int)ceil(log10((double)totalIter));
    for (int i = 0; i < totalIter; i++) {
        char outFileIndexed[512];
        snprintf(outFileIndexed, sizeof(outFileIndexed), "%s_%0*d.wrl", outFile, digits, i);
        FILE* fout = fopen(outFileIndexed, "w");
        if (!fout) {
            fclose(fin);
            mexErrMsgTxt("Error cannot open output file");
        }
        double rotationAngle = i * angleDegDelta;
        //resets
        fseek(fin, 0, SEEK_SET);
        printfiles(fin, fout, rotationAngle);
        fclose(fout);

    }




    fclose(fin);
    return;
}

//write function that calls file printing, also restructure so main doesnt include file IO, make seperate func
//main should only handle args and file opening

void printfiles(FILE* fin, FILE* fout, double degreesDelta) {
    //gets scalar angle
    double theta = degreesDelta * M_PI / 180.0;
    double c = cos(theta);
    double s = sin(theta);
    char buf[256];
    bool inPoints = false;
    while (fgets(buf, sizeof(buf), fin)) {
        //processes each coord 
        if (inPoints) {
            //Do no further processing of coords if ] detected.
            if (strchr(buf, ']')) {
                inPoints = false;
                fprintf(fout, "]\n");
            }
            else {
                double x;
                double y;
                double z;
                int offset;
                char* ptr = buf;
                while (sscanf(ptr, " %lf %lf %lf ,%n", &x, &y, &z, &offset) == 3 || sscanf(ptr, "%lf %lf %lf%n", &x, &y, &z, &offset) == 3) {
                    double x2 = x * c - y * s;
                    double y2 = x * s + y * c;
                    fprintf(fout, " %.6f %.6f %.6f,", x2, y2, z);
                    ptr += offset;
                }
                fprintf(fout, "\n");
            }
        }
        //finds line with point in it and then advances to next line
        else if (strstr(buf, "point")) {
            fputs(buf, fout);
            inPoints = true;
        }
        //prints all else to file without change
        else {
            fputs(buf, fout);
        }
    }
}