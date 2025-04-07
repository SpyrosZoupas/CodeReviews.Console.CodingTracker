using CodingTracker;
using CodingTracker.DAL;

CodingTrackerRepository codingTrackerRepository = new CodingTrackerRepository();
CodingSessionController codingSessionController = new CodingSessionController(codingTrackerRepository);
Validation validation = new Validation(codingSessionController);
UserInput codingTrackerApp = new UserInput(codingSessionController, validation);
codingSessionController.CreateTables();
codingTrackerApp.GetUserInput();

