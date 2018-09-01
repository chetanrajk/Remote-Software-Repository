/////////////////////////////////////////////////////////////////////
// Browse.cpp - Implements Browsing on Software repository          //
// ver 1.0                                                         //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018   //
/////////////////////////////////////////////////////////////////////

#include "Browse.h"

#ifdef TEST_BROWSE
int main()
{
	DbCore<PayLoad> db;
	PayLoad pl;
	DbElement<PayLoad> elem;

	elem.name("Executive.h");
	elem.descrip("John");
	pl.value() = "../NoSqlDb (1)/Executive/Executive.h";
	pl.categories().push_back("Header file");
	pl.categories().push_back("Execution");
	elem.payLoad(pl);
	elem.addChildKey("NoSqlDb::DbCore.h");
	elem.addChildKey("NoSqlDb::PayLoad.h");
	db["::Executive.h"] = elem;
	pl.categories().clear();
	elem.children().clear();

	elem.name("DbCore.h");
	elem.descrip("Parker");
	pl.value() = "../NoSqlDb (1)/DbCore/DbCore.h";
	pl.categories().push_back("Header file");
	pl.categories().push_back("Functionality");
	elem.payLoad(pl);
	elem.addChildKey("NoSqlDb::PayLoad.h");
	db["NoSqlDb::DbCore.h"] = elem;
	pl.categories().clear();
	elem.children().clear();


	Browse bObj;
	bObj.browseDb(db);

	return 0;
}
#endif