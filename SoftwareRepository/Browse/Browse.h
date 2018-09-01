#pragma once
/////////////////////////////////////////////////////////////////////
// Browse.h - Implements Browsing on Software repository            //
// ver 1.0                                                         //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018   //
/////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* This package provides Browsing on Software repository
* -
* Required Files:
* ---------------
* Query.h , DbCore.h , PayLoad.h
*
* Maintenance History:
* --------------------
* ver 1.0 : 25 Feb 2018
* - first release
*/
#include "../NoSqlDb (1)/DbCore/DbCore.h"
#include "../Payload/PayLoad.h"
#include "../Process/Process/Process.h"
#include "../NoSqlDb (1)/Query/Query.h"

using namespace NoSqlDb;

class Browse
{
public:
	Keys browseDb(DbCore<PayLoad>& db, std::string qFileName, std::string qCategory, std::string qDependency, std::string qVersion, std::string qParent);
	inline Keys versionCheck(DbCore<PayLoad>& db, std::string qFileName, std::string qVersion, Query<PayLoad> q1);
	inline Keys categoryCheck(DbCore<PayLoad>& db, std::string qCategory, Query<PayLoad> q3);
};

//----< fuction for browseDb >---------------------------------------------

inline Keys Browse::browseDb(DbCore<PayLoad>& db, std::string qFileName, std::string qCategory, std::string qDependency, std::string qVersion, std::string qParent)
{
	Keys result, resultDep;
	Query<PayLoad> q1(db), q2(db), q3(db), q4(db), q5(db);
	Conditions<PayLoad> conds1, conds2, conds3, conds4, condsCh;
	if (!qParent.empty())
	{
		result.clear();
		for (Key key1 : db.keys())
		{
			Keys keys{ key1 }; conds4.children(keys);
			if ((q5.select(conds4).keys()).empty())
				result.push_back(db[key1].name());
		}
	}
	if (!qDependency.empty())
	{
		result.clear();	conds3.name(qDependency);
		Keys keys{ q4.select(conds3).keys() };
		if (!keys.empty())
		{
			condsCh.children(keys);
			for (Key key : q4.select(condsCh).keys())
				result.push_back(db[key].name());
		}
	}
	if (!qCategory.empty())
	{
		result.clear();
		result = categoryCheck(db, qCategory, q3);
	}
	if (!qFileName.empty())
	{
		result.clear(); conds1.name(qFileName); q1.select(conds1);
		for (Key key : q1.keys())
			result.push_back(db[key].name());
	}
	if (!qVersion.empty())
	{
		result.clear();
		result = versionCheck(db, qFileName, qVersion, q1);
	}
	return result;
}

//----< fuction for category matching >---------------------------------------------

inline Keys Browse::categoryCheck(DbCore<PayLoad>& db, std::string qCategory, Query<PayLoad> q3)
{
	Keys result;
	auto hasCategory = [&qCategory](DbElement<PayLoad>& elem) {
		return (elem.payLoad()).hasCategory(qCategory);
	};
	for (Key key : q3.select(hasCategory).keys())
		result.push_back(db[key].name());
	return result;
}
//----< fuction for version matching >---------------------------------------------

inline Keys Browse::versionCheck(DbCore<PayLoad>& db, std::string qFileName, std::string qVersion, Query<PayLoad> q1)
{
	Keys result;
	Query<PayLoad> q2(db);
	Conditions<PayLoad> conds1, conds2;
	std::string regEx = "(.*" + qVersion + ")";
	conds2.name(regEx);
	q2.select(conds2).keys();
	for (Key key : q2.keys())
	{
		if (!qFileName.empty())
		{
			if (std::find(q1.keys().begin(), q1.keys().end(), key) != q1.keys().end())
				result.push_back(db[key].name());
		}
		else
		{
			result.push_back(db[key].name());
		}
	}
	return result;
}
