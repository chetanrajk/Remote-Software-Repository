#pragma once
/////////////////////////////////////////////////////////////////////
// CheckOut.h - Implements Check-out functionality                 //
//									on Software repository         //
// ver 1.0                                                         //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018   //
/////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* This package provides Check-Out functionality on Software repository
* -
* Required Files:
* ---------------
*  Version.h , DbCore.h , PayLoad.h
*
* Maintenance History:
* --------------------
* ver 1.0 : 25 Feb 2018
* - first release
*/

#include <iostream>
#include <fstream>
#include "../NoSqlDb (1)/DbCore/DbCore.h"
#include "../Payload/PayLoad.h"
#include "../Version/Version.h"
#include <string>
#include <sstream>
#include "../../FileSystem-Windows/FileSystemDemo/FileSystem.h"

using namespace NoSqlDb;

class CheckOut
{
public:
	void checkOutFile(DbCore<PayLoad>& db, std::string pkg, std::string file, std::string srcpath, std::string dstpath);
};

//----< fuction for checkOut provided file >---------------------------------------------

void CheckOut::checkOutFile(DbCore<PayLoad>& db, std::string pkg, std::string file, std::string srcpath, std::string dstpath)
{
	std::cout << "\n\n Provided details for file to check-out:";
	std::cout << "\n\n Package name:  " << pkg;
	std::cout << "\n File name:  " << file;
	std::cout << "\n Local Path:  " << srcpath;

	std::string newfilename = file.substr(0, file.length() - 2);
	std::string newdstpath = dstpath.substr(0, dstpath.length() - 2);
	FileSystem::File::copy(srcpath, newdstpath);

	std::cout << "\n\n File has been checked out as :" << newfilename << "   ...(Please check Local Storage folder)";

	return;
}