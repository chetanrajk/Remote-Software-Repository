#pragma once
/////////////////////////////////////////////////////////////////////
// CheckIn.h - Implements Check-in functionality                   //
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
* Version.h , DbCore.h , PayLoad.h 
*
* Maintenance History:
* --------------------
* ver 1.0 : 25 Feb 2018
* - first release
*/

#include <iostream>
#include <fstream>
#include "../NoSqlDb (1)/DbCore/DbCore.h"
#include "../../FileSystem-Windows/FileSystemDemo/FileSystem.h"
#include "../../ServerPrototype/ServerPrototype.h"
#include "../Payload/PayLoad.h"
#include "../Version/Version.h"
#include <string>
#include <sstream>

using namespace NoSqlDb;

class CheckIn
{
public:
	void checkInFile(DbCore<PayLoad>& db, std::string pkg, std::string file, std::string auth, Keys dep, std::string srcpath, std::string dstpath, Keys cat, std::string status);
	void newFile(Key key,DbCore<PayLoad>& db, std::string pkg, std::string file, std::string auth, Keys dep, std::string srcpath, std::string dstpath, Keys cat, std::string status);
	void openFile(Key key, DbCore<PayLoad>& db, std::string pkg, std::string file, std::string auth, Keys dep, std::string srcpath, std::string dstpath, Keys cat, std::string status);
	void closeFile(Key key, DbCore<PayLoad>& db, std::string pkg, std::string file, std::string auth, Keys dep, std::string srcpath, std::string dstpath, Keys cat, std::string status);
};


//----< fuction for checkIn file >---------------------------------------------
inline void CheckIn::checkInFile(DbCore<PayLoad>& db, std::string pkg, std::string file, std::string auth, Keys dep, std::string srcpath, std::string dstpath, Keys cat, std::string status)
{
	std::cout << "\n\n Provided details for file to check-in:";
	std::cout << "\n\n Package name:  "<<pkg;
	std::cout << "\n File name:  "<<file;
	std::cout << "\n Author:  "<<auth;
	std::cout << "\n Dependencies:  ";
	for (auto key : dep)
		std::cout << key << ", ";
	std::cout << "\n Status:  " << status;
	std::cout << "\n Dependencies:  ";
	for (auto key : cat)
		std::cout << key << ", ";
	
	Version verObj;
	Key key = pkg + "::" + file;
	int latestver = verObj.latestFileVersion(db,key);
	key = key + "." + ((char)latestver);

	if (!db.contains(key))
	{
		newFile(key, db, pkg, file, auth, dep, srcpath,dstpath,cat,status);
	}
	else
	{
		if (db[key].descrip() != auth)
		{
			std::cout << "\n\n Person trying to check-in is not owner of the file.";
			std::cout << "\n\n Check-in denied !!";
		}
		else
		{
			if (db[key].payLoad().status() == "Open")
			{
				openFile(key, db, pkg, file, auth, dep, srcpath, dstpath, cat, status);

			}
			else
			{
				closeFile(key, db, pkg, file, auth, dep, srcpath, dstpath, cat, status);
			}
		}
	}
	return;
}

//----< fuction for checkIn file and create new file >---------------------------------------------

inline void CheckIn::newFile(Key key,DbCore<PayLoad>& db, std::string pkg, std::string file, std::string auth, Keys dep, std::string srcpath, std::string dstpath, Keys cat, std::string status)
{
	file = file + ".1";
	dstpath = dstpath + ".1";
	std::string fullDstPath = FileSystem::Path::getFullFileSpec(dstpath);
	FileSystem::File::copy(srcpath, fullDstPath);

	PayLoad pl;
	DbElement<PayLoad> elem;
	elem.name(file);
	elem.descrip(auth);
	pl.value() = dstpath;
	pl.categories()=cat;
	pl.status(status);
	elem.payLoad(pl);
	for (Key key : dep)
		elem.addChildKey(key);
	db[key] = elem;
	pl.categories().clear();
	elem.children().clear();

	std::cout << "\n\n New file has been Checked-In to repository..(Please check Storage folder)";
	std::cout << "\n\n Metadata has been updated:\n";
	showDb(db);
	elem.payLoad().showDb(db);
	return;
}

//----< fuction for checkIn open file >---------------------------------------------

inline void CheckIn::openFile(Key key, DbCore<PayLoad>& db, std::string pkg, std::string file, std::string auth, Keys dep, std::string srcpath, std::string dstpath, Keys cat, std::string status)
{
	Version verObj;
	int oldver = verObj.fileVersion(db, key,false);
	file = file + "." + ((char)oldver);
	dstpath = dstpath + "." + ((char)oldver);

	std::string fullDstPath = FileSystem::Path::getFullFileSpec(dstpath);
	FileSystem::File::copy(srcpath, fullDstPath);

	PayLoad pl;
	DbElement<PayLoad> elem;
	elem.name(file);
	elem.descrip(auth);
	pl.value() = dstpath;
	pl.categories() = cat;
	pl.status(status);
	elem.payLoad(pl);
	for (Key key : dep)
		elem.addChildKey(key);
	db[key] = elem;
	pl.categories().clear();
	elem.children().clear();

	std::cout << "\n\n Existing file has been overwritten: " << file << "..(Please check Storage folder)";
	std::cout << "\n\n Metadata has been updated:\n";
	showDb(db);
	elem.payLoad().showDb(db);
	return;
}

//----< fuction for checkIn closed file >---------------------------------------------

inline void CheckIn::closeFile(Key key, DbCore<PayLoad>& db, std::string pkg, std::string file, std::string auth, Keys dep, std::string srcpath, std::string dstpath, Keys cat, std::string status)
{

	Version verObj;
	int oldver = verObj.fileVersion(db, key,false);
	int newVer = verObj.fileVersion(db, key,true);
	file = file + "." + ((char)oldver);
	file.back() = (char)newVer;
	key.back() = (char)newVer;;
	Key newkey = key;

	dstpath = dstpath + "." + ((char)newVer);
	std::string fullDstPath = FileSystem::Path::getFullFileSpec(dstpath);
	FileSystem::File::copy(srcpath, fullDstPath);

	PayLoad pl;
	DbElement<PayLoad> elem;
	elem.name(file);
	elem.descrip(auth);
	pl.value() = dstpath;
	pl.categories() = cat;
	pl.status(status);
	elem.payLoad(pl);
	for (Key key : dep)
		elem.addChildKey(key);
	db[newkey] = elem;
	pl.categories().clear();
	elem.children().clear();

	std::cout << "\n\n New version of file has been created :" << file <<"   ..(Please check Storage folder)";
	std::cout << "\n\n Metadata has been updated:\n";
	showDb(db);
	elem.payLoad().showDb(db);
	return;
}