#include "mex.h"
#include <cmath>
// Written by Steven G. Lewis, 2025
// University of Maryland - College Park
// Gemstone Team RUBIX


//============================================================================
//This MEX file processes a single triangle and projects it
//from 3D space into a 2D coordinate system aligned with a given direction vector.
//The resulting 2D triangle representation is intended for drag
//analysis such as projected area and surface interaction calculations.
//
//High level flow:
//1.Read a 3D triangle and a direction (velocity) vector from MATLAB.
//2.Normalize the direction vector to form the primary axis.
//3.Construct an orthonormal basis using cross products.
//4.Translate the triangle so one vertex lies at the origin.
//5.Project the remaining vertices onto the plane perpendicular to the
//   direction vector using dot products.
//6. Return the flattened 2D coordinates
//
//Mathematical operation:
//Let I be the normalized direction vector. Two orthogonal unit vectors J and K
//are constructed such that {I, J, K} forms an orthonormal basis.
//For each vertex v relative to A, compute its 2D coordinates as:
//u = v · J
//v = v · K
//This produces a projection of the triangle onto the plane normal
//to the input direction vector.
//==========================================================================

void mexFunction(int nlhs, mxArray *plhs[], int nrhs, const mxArray *prhs[])
{
    //Make sure at least 3 inputs were passed from MATLAB
    if (nrhs < 3)
        mexErrMsgTxt("Usage: surfacegeometry_mx(tri3D, N, Vdir, flag)");

    //============Input Handling=================
    //tri3D contains the triangle vertices in 3D:
    //[x1 y1 z1 x2 y2 z2 x3 y3 z3]
    double *tri3D = mxGetPr(prhs[0]);

    //Velocity vector that defines the viewing/projection direction
    double *Vdir= mxGetPr(prhs[2]);

    //Extract triangle points A, B, C from the tri3D array
    double A[3] = { tri3D[0], tri3D[1], tri3D[2] };
    double B[3] = { tri3D[3], tri3D[4], tri3D[5] };
    double C[3] = { tri3D[6], tri3D[7], tri3D[8] };

    //=============Normalize The Velocity===========
    //Compute magnitude of Vdir
    double Vmag = sqrt(Vdir[0]*Vdir[0] + Vdir[1]*Vdir[1] + Vdir[2]*Vdir[2]);

    //Prevent dividing zero if a zero vector is passed
    if (Vmag == 0) Vmag = 1;

    //I_hat becomes a unit vector in the velocity direction
    double I_hat[3] = { Vdir[0]/Vmag, Vdir[1]/Vmag, Vdir[2]/Vmag };

    //=======Orthogonal Coordinate System=========
    //Create a temporary vector that is NOT parallel to I_hat.
    //Used to generate perpendicular vectors via cross product.
    double tmp[3] = {1, 0, 0};

    //If I_hat is too aligned with x-axis, switch tmp to y-axis
    //to avoid a near-zero cross product.
    if (fabs(I_hat[0]) > 0.9) { tmp[0] = 0; tmp[1] = 1; tmp[2] = 0; }

    //Compute J_hat = I_hat x tmp (cross product)
    //This gives a vector perpendicular to the velocity direction.
    double J_hat[3] = {
        I_hat[1]*tmp[2] - I_hat[2]*tmp[1],
        I_hat[2]*tmp[0] - I_hat[0]*tmp[2],
        I_hat[0]*tmp[1] - I_hat[1]*tmp[0]
    };

    //Normalize J_hat so it becomes a unit vector
    double Jmag = sqrt(J_hat[0]*J_hat[0] + J_hat[1]*J_hat[1] + J_hat[2]*J_hat[2]);
    for (int i = 0; i < 3; ++i) J_hat[i] /= Jmag;

    //Compute K_hat = I_hat x J_hat
    //Now we have a full orthonormal basis (I, J, K)
    double K_hat[3] = {
        I_hat[1]*J_hat[2] - I_hat[2]*J_hat[1],
        I_hat[2]*J_hat[0] - I_hat[0]*J_hat[2],
        I_hat[0]*J_hat[1] - I_hat[1]*J_hat[0]
    };

    //==========Project Triangle To 2D========
    //Shift triangle so point A becomes the origin.
    //This simplifies projection math.
    double B_rel[3] = { B[0]-A[0], B[1]-A[1], B[2]-A[2] };
    double C_rel[3] = { C[0]-A[0], C[1]-A[1], C[2]-A[2] };

    //A is now at (0,0) in 2D space
    double A2D[2] = {0,0};

    //Project B onto the plane formed by J_hat and K_hat
    //using dot products.
    double B2D[2] = {
        B_rel[0]*J_hat[0] + B_rel[1]*J_hat[1] + B_rel[2]*J_hat[2],
        B_rel[0]*K_hat[0] + B_rel[1]*K_hat[1] + B_rel[2]*K_hat[2]
    };

    //Same projection for point C
    double C2D[2] = {
        C_rel[0]*J_hat[0] + C_rel[1]*J_hat[1] + C_rel[2]*J_hat[2],
        C_rel[0]*K_hat[0] + C_rel[1]*K_hat[1] + C_rel[2]*K_hat[2]
    };

    //==========Output========
    //Allocate a 1x6 MATLAB array for the projected triangle
    //[Ax Ay Bx By Cx Cy]
    plhs[0] = mxCreateDoubleMatrix(1, 6, mxREAL);
    double *projectedTri = mxGetPr(plhs[0]);

    //Store the projected coordinates
    projectedTri[0] = A2D[0]; projectedTri[1] = A2D[1];
    projectedTri[2] = B2D[0]; projectedTri[3] = B2D[1];
    projectedTri[4] = C2D[0]; projectedTri[5] = C2D[1];

    //========Placeholder Outputs=========
    //These exist so the function signature stays compatible
    //with older code that expects multiple outputs.

    if (nlhs > 1)
        plhs[1] = mxCreateDoubleMatrix(1, 1, mxREAL);

    if (nlhs > 2)
        plhs[2] = mxCreateDoubleMatrix(1, 1, mxREAL);
}