#pragma once
/////////////////////////////////////////////////////////////////////
// Version.h - Implements Version functionality                    //
//									on Software repository         //
// ver 1.0                                                         //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018   //
/////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* This package provides Check-in functionality on Software repository
* -
* Required Files:
* ---------------
* DbCore.h , PayLoad.h
*
* Maintenance History:
* --------------------
* ver 1.0 : 25 Feb 2018
* - first release
*/
#include <iostream>
#include "../NoSqlDb (1)/DbCore/DbCore.h"
#include "../Payload/PayLoad.h"
#include <string>

using namespace NoSqlDb;

class Version
{
public:
	int fileVersion(DbCore<PayLoad>& db, Key key, bool increment);
	int latestFileVersion(DbCore<PayLoad>& db, Key key);
};

//----< fuction for getting file version file >---------------------------------------------

int Version::fileVersion(DbCore<PayLoad>& db, Key key, bool increment)
{
	int newVer;

	newVer = (int)(db[key].name().back());

	if (increment)
	{
		return newVer + 1;
	}

	return newVer;
}

//----< fuction for getting latest version of file >---------------------------------------------

int Version::latestFileVersion(DbCore<PayLoad>& db, Key key)
{
	int latestVer;
	int max = 49;
	Keys keys = db.keys();

	for (Key key1 : keys)
	{
		if (key1.find(key) != std::string::npos)
		{
			int ver = (int)(db[key1].name().back());
			if (ver > max)
			{
				max = ver;
			}
		}
	}
	latestVer = max;
	return latestVer;
}