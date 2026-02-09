%%%%TODO
%turn into function
%%%%%%%%%%%%%%

% Written by Steven G. Lewis, 2025
% University of Maryland - College Park
% Gemstone Team RUBIX
%
%=====================================================================
%Loads a .mat file containing a 4D coefficient matrix, then computes the
%average coefficient value and average surface value by iterating through
%every element of the cube defined by degreeNumber.
%
%High-level flow - 
%1.Load simulation data into memory.
%2.Initialize running sums for coefficients and surface values.
%3.Accumulate values from the 4th dimension (channels 1 and 2).
%4.Obtain averages.
%

load('storedCoeffData\UPDATEDsimplesat1U.mat');

%==========Initialize Running Totals==========
sumofCoeff = 0;
sumofSurface = 0;

%===============Traverse 3D Matrix===============
for i = 1:degreeNumber
    for j = 1:degreeNumber
        for k = 1:degreeNumber
            
            %Accumulate coefficient
            sumofCoeff = sumofCoeff + coeffSurfaceMatrix(i,j,k,2);
            
            %Accumulate surface area
            sumofSurface = sumofSurface + coeffSurfaceMatrix(i,j,k,1);
        end
    end
end

%=============Compute Total Sample Count==================
values = degreeNumber * degreeNumber * degreeNumber;

%==============Calculate Arithmetic Means===========
sumofCoeff = sumofCoeff / values
sumofSurface = sumofSurface / values