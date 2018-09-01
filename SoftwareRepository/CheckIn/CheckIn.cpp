/////////////////////////////////////////////////////////////////////
// CheckIn.cpp - Implements Checkin on Software repository         //
// ver 1.0                                                         //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018   //
/////////////////////////////////////////////////////////////////////

#include "CheckIn.h"

#ifdef TEST_CHECKIN
int main()
{
	DbProvider dbp;
	DbCore<PayLoad> db = dbp.db();

	CheckIn chkIn;
	Keys dep;
	std::string path;

	std::cout << "\n\n----> Case 1: File is present and in Open status:";
	dep.push_back("DbCore::DbCore.h.1");
	path = "../Local/PayLoad/PayLoad.h.1";
	chkIn.checkInFile(db, "PayLoad", "PayLoad.h.1", "Robert", dep, path);
	dep.clear();

	std::cout << "\n\n----> Case 2: File not present present and so new entry";
	path = "../Local/DemoPkg/DemoFile.h.1";
	chkIn.checkInFile(db, "DemoPkg", "DemoFile.h.1", "XYZ", dep, path);
	dep.clear();

	return 0;
}
#endif