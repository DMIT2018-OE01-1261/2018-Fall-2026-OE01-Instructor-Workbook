using BYSResults;

namespace OLTPSystem.Helpers;

public static class ResultHelpers
{
	// Converts a list of error objects into their string representations.
	//	Each string looks like "Code: Message",
	//	e.g. "Missing Information: A valid CustomerID must be provided."
	//	Used by the tests now and by your Blazor pages later.
	public static List<string> GetErrorMessages(IEnumerable<Error> errors)
	{
		// Initialize a new list to hold the extracted error messages
		List<string> errorList = new();

		// Iterate over each Error object in the incoming list
		foreach (var error in errors)
		{
			// Convert the current Error to its string form and add it to errorList
			errorList.Add(error.ToString());
		}

		// Return the populated list of error message strings
		return errorList;
	}
}
