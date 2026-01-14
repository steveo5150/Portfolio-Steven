# Atmospheric/CD calculations for infalatable nanosatellite

Current code used to gather data for designing inflatable cubesat. Gathers data and compares a spherical satellite to an inflatable one. Displays certain datapoints based on what is required. Can easily be changed to display different data and comparisons. Utilizes .wrl files as input, any shape created in a 3D modeling software can be run through this code.

## Getting Started

Simply download matlab, and install all files. Initially run the ModelRun_Main. This file can be altered to change results. More info on how to handle altering data inputs and outputs can be found wihtin code files.

### Prerequisites

matlab

## IMPORTANT DETAILS
currently the code works to reduce run time after the initial run, creates files to be read opposed to altering calculations each time while still reading a file. This sacrifices space and thus files should be deleted after use. Such files will currently be located in CUBEMODELS folder, any files with a suffix of numbers can be deleted
