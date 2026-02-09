%%%%%%%%%TODO:%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%5
%add comments, turn into function

%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%Simple graph of drag force W.R.T. radius of a sphere
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%Can be easily changed to also model error in surface area or force by

%INPUTS%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

diaMin = 0;
diaMax = 1;
diaInterval = .1;

diaRuns = floor((diaMax - diaMin) / diaInterval) + 1
allForce = zeros(1, diaRuns);
allDiam = zeros(1, diaRuns);
allSurface = zeros(1, diaRuns);
function [forces, surfaces, diameters] = runSphereForce(min, max, interval, runs)
    forces = zeros(1, runs);
    surfaces = zeros(1, runs);
    diameters = zeros(1, runs);
    for i = 1:runs
        diaCurrent = min + ((i-1) * interval);
        [~, ~, SURFACE, FORCE] = MAIN(1, diaCurrent, 2.01, 3.4, 0, 12.5, ...
            1200.5, 7800.45, 100000000000, 1000000, 1000000, 1000000, ...
            10000, 0, .93, 65, [], "");
        forces(i) = FORCE;
        surfaces(i) = SURFACE;
        diameters(i) = diaCurrent;
    end
end
[allForce, allSurface, allDiam] = runSphereForce(diaMin, diaMax, diaInterval, diaRuns);
figure;
plot(allDiam, allForce, 'b', 'LineWidth', 2);
grid on; box on;

title('Drag Force vs Sphere Diameter');
xlabel('Diameter');
ylabel('Drag Force');

legend('Drag Force','Location','best');


figure;
plot(allSurface, allForce, 'r', 'LineWidth', 2);
grid on; box on;

title('Drag Force vs Sphere Surface Area');
xlabel('Surface Area');
ylabel('Drag Force');

legend('Drag Force','Location','best');