%%%%%%%%%TODO:%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%add comments, turn into function




%write minimum gathering function for Cd/surface area combo
%Gather info on minimum:
%   DENSITY
%   SPEED
%Gather info and calculate minimum acceleration threshhold.
%   a. Check current accuracy of models - what would be better
%   b. reference NOISE table/calculation in combination with error of
%       design to sphere - derive from VECTOR(MAIN) & AvgStoredValue...
%       to determine acceleration.
%   BetterPercent - spherepercent = allowableNoiseError
%   allowableNoiseError used to backtrack acceleration with EQ

%create plot of exact mass with certain radii to gather what maximum mass
%   satellite can have - approximate to sphere of that size

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


% Written by Steven G. Lewis, 2025
% University of Maryland - College Park
% Gemstone Team RUBIX


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%Simple graph of drag force W.R.T. radius of a sphere
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

%INPUTS%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% 
%alter these to change what diameters to test
diaMin = 0;
diaMax = 1;
diaInterval = .1;
global filename atmosphericDensity velocity shape acceleration dv
%change for geometry file
filename = 0;
%set based on minimum atmospheric density
atmosphericDensity = 1;
%set based on minimum orbit velocity
velocity = 1;

dv = velocity * atmosphericDensity;

%change 1 for sphere, anything else for geometry file
shape = 1;
%change desired acceleration based on feel the noise paper
acceleration = 1;


%ballistic coefficient / Acceleration = mass - plot this
diaRuns = floor((diaMax - diaMin) / diaInterval) + 1;
%allocate mass diam and surface arrays
allMass = zeros(1, diaRuns);
allDiam = zeros(1, diaRuns);
allSurface = zeros(1, diaRuns);





%=============== Functions =================================================
%calculate Surface Area and Cd according to VECTOR
function [SurfaceArea, DragCoeff] = getSaCd(diameter, shape)
    if shape == 1
        [~, DragCoeff, SurfaceArea, ~] = MAIN(1, diameter, 2.01, 3.4, 0, 12.5, 1200.5, 7800.45, 100000000000, 1000000, 1000000, 1000000, 10000, 0, .93, 65, [], "");
    else
        [~, DragCoeff, SurfaceArea, ~] = MAIN(4, diameter, 2.01, 3.4, 0, 12.5, 1200.5, 7800.45, 100000000000, 1000000, 1000000, 1000000, 10000, 0, .93, 65, [], "");
    end
end

%calculate density based on composition

%calculate ballisitc Coefficient
function [ballisticCoeff] = getballistic(Cd, Sa)
    global dv
    ballisticCoeff = dv * Cd * Sa * .5;
end
%calculates required maximum mass
function [surfaceArea, mass] = getSamass(diaCurrent)
    global shape acceleration
    [surfaceArea, dragCoefficient] = getSaCd(diaCurrent, shape);
    ballistic = getballistic(dragCoefficient, surfaceArea);
    mass = ballistic / acceleration;
end

% calculates minimum Cd for shape, then calculates surface area, to find
% surface subtract sphere from original file, then just continually
% increase surface area
function [minCoeff, minSA] = getminCoeffSA()
for 
end


%begin MAIN ==============================================================
for i = 1:diaRuns
    diaCurrent = diaMin + ((i-1) * diaInterval);
    allDiam(i) = diaCurrent;
    [allSurface(i), allMass(i)] = getSamass(diaCurrent);
end





%begin PLOTS =============================================================
figure;
plot(allDiam, allMass, 'b-o', 'LineWidth', 2);
grid on; box on;

title('Required Mass vs Diameter for Target Drag Acceleration');
xlabel('Diameter');
ylabel('Required Mass');

legend(sprintf('Target acceleration = %.2f', acceleration), 'Location','best');

figure;
plot(allSurface, allMass, 'r-o', 'LineWidth', 2);
grid on; box on;

title('Required Mass vs Surface Area for Target Drag Acceleration');
xlabel('Surface Area');
ylabel('Required Mass');

legend(sprintf('Target acceleration = %.2f', acceleration), 'Location','best');