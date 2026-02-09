%%%%%%%%%TODO:%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%add comments, turn into function

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%



% Written by Steven G. Lewis, 2025
% University of Maryland - College Park
% Gemstone Team RUBIX

%!!CAUTION!! using an existing filename WILL CORRUPT FILES !!CAUTION!!
global inputfile
inputfile = "1Usat/1UcubesimpleUpdate"; %might change to just overwrite in wrlrotator
%!!CAUTION!! using an existing filename WILL CORRUPT FILES !!CAUTION!!

%File Format Details
%When using VRML 2.0/wrl 2.0, edit file as so:
%where points start, ensure numbers occur on a different line from point [
%do the same for coordIndex [, and also remove coord keeping only index
%Also, remove spacing by finding and replacing two spaces with nothing
%Finally, ensure ending brackets ] are on a seperate line from the final
%point/ index
%Also - ensure passed in file is one solid object, to avoid inaccurate data
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


%total degrees to be rotated upon, 90 goes from 0-90, 45 goes from 0-45, etc.
global totalDegrees degreeDelta
totalDegrees = 90;  % Degrees from 0-90 to iterate over
degreeDelta = 10;   % Change in degrees per rotation
degreeNumber = floor(totalDegrees / degreeDelta);
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
global fileNumber digits
fileNumber = totalDegrees/degreeDelta
digits = ceil(log10(fileNumber))
fin = sprintf("%s.wrl", inputfile);
fout = inputfile;
% --- Get script folder ---
scriptFolder = pwd;

% --- Input and output files ---
fin  = char(fullfile(scriptFolder, inputfile + '.wrl'));       % absolute path to input WRL
fout = char(fullfile(scriptFolder, inputfile)); % absolute path to output WRL
%creates files for input
databasefilename = "UPDATEDsimplesat1U.mat";
wrlrotator_mx(fin,fout,degreeDelta,totalDegrees);

function [coeffsurface] = GetCoeffSurface(Degrees)
    global totalDegrees degreeDelta fileNumber inputfile digits
    coeffsurface = zeros(Degrees, Degrees, Degrees, 2);
    %random degree from 1-45, if you want to model different objects with
    %less symmetry then a cube, increase to higher value (90, 180, 360)
    for pitch = 0:Degrees-1
        for sideslip = 0:Degrees-1
            for yaw = 0:Degrees-1
                %iterate through files, choosing whichever mathches desired yaw
                %change file names if you want to model a different shape.
                fileName = sprintf("%s_%0*d.wrl",inputfile, digits, yaw);
                %fileName = sprintf("%s.wrl",inputfile);
                pitch
                sideslip
                yaw    
                [~, COEFF, SURFACE,~] = MAIN(4, 1.212, 2.01, 3.4, ...
                    pitch * degreeDelta, sideslip * degreeDelta, 1200.5, ...
                    7800.45, 100000000000, 1000000, 1000000, 1000000, ...
                    10000, 0, .93, 65, [], fileName);
                SURFACE
                COEFF
                %1-7-1
                coeffsurface(pitch+1, yaw+1, sideslip+1, 1) = SURFACE;
                coeffsurface(pitch+1, yaw+1, sideslip+1, 2) = COEFF;
            end
        end
    end
end

coeffSurfaceMatrix = GetCoeffSurface(degreeNumber);
save(databasefilename,"coeffSurfaceMatrix", "degreeNumber");