#include "mex.h"
#include <cmath>

// Returns 3 outputs: PAM, placeholder1, placeholder2

void mexFunction(int nlhs, mxArray *plhs[], int nrhs, const mxArray *prhs[])
{
    if (nrhs < 3)
        mexErrMsgTxt("Usage: surfacegeometry_mx(TRI, N, Vdir, flag)");

    //Inputs handling
    double *TRI = mxGetPr(prhs[0]);   // [x1 y1 z1 x2 y2 z2 x3 y3 z3]
    double *Vdir= mxGetPr(prhs[2]);   // velocity vector

    double A[3] = { TRI[0], TRI[1], TRI[2] };
    double B[3] = { TRI[3], TRI[4], TRI[5] };
    double C[3] = { TRI[6], TRI[7], TRI[8] };
    //Normalize Vdir
    double Vmag = sqrt(Vdir[0]*Vdir[0] + Vdir[1]*Vdir[1] + Vdir[2]*Vdir[2]);
    if (Vmag == 0) Vmag = 1;
    double I_hat[3] = { Vdir[0]/Vmag, Vdir[1]/Vmag, Vdir[2]/Vmag };
    //Compute orthogonal unit vectors
    double tmp[3] = {1, 0, 0};
    if (fabs(I_hat[0]) > 0.9) { tmp[0] = 0; tmp[1] = 1; tmp[2] = 0; }
    double J_hat[3] = {
        I_hat[1]*tmp[2] - I_hat[2]*tmp[1],
        I_hat[2]*tmp[0] - I_hat[0]*tmp[2],
        I_hat[0]*tmp[1] - I_hat[1]*tmp[0]
    };
    double Jmag = sqrt(J_hat[0]*J_hat[0] + J_hat[1]*J_hat[1] + J_hat[2]*J_hat[2]);
    for (int i = 0; i < 3; ++i) J_hat[i] /= Jmag;
    double K_hat[3] = {
        I_hat[1]*J_hat[2] - I_hat[2]*J_hat[1],
        I_hat[2]*J_hat[0] - I_hat[0]*J_hat[2],
        I_hat[0]*J_hat[1] - I_hat[1]*J_hat[0]
    };
    //Project into 2D
    double B_rel[3] = { B[0]-A[0], B[1]-A[1], B[2]-A[2] };
    double C_rel[3] = { C[0]-A[0], C[1]-A[1], C[2]-A[2] };

    double A2D[2] = {0,0};
    double B2D[2] = {
        B_rel[0]*J_hat[0] + B_rel[1]*J_hat[1] + B_rel[2]*J_hat[2],
        B_rel[0]*K_hat[0] + B_rel[1]*K_hat[1] + B_rel[2]*K_hat[2]
    };
    double C2D[2] = {
        C_rel[0]*J_hat[0] + C_rel[1]*J_hat[1] + C_rel[2]*J_hat[2],
        C_rel[0]*K_hat[0] + C_rel[1]*K_hat[1] + C_rel[2]*K_hat[2]
    };
    //Output PAM (6 values)
    plhs[0] = mxCreateDoubleMatrix(1, 6, mxREAL);
    double *PAM = mxGetPr(plhs[0]);
    PAM[0] = A2D[0]; PAM[1] = A2D[1];
    PAM[2] = B2D[0]; PAM[3] = B2D[1];
    PAM[4] = C2D[0]; PAM[5] = C2D[1];
    //Output placeholder zeros
    if (nlhs > 1)
        plhs[1] = mxCreateDoubleMatrix(1, 1, mxREAL);
    if (nlhs > 2)
        plhs[2] = mxCreateDoubleMatrix(1, 1, mxREAL);
}