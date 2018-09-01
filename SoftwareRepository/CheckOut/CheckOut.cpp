/////////////////////////////////////////////////////////////////////
// CheckOut.cpp - Implements Checkout on Software repository        //
// ver 1.0                                                         //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018   //
/////////////////////////////////////////////////////////////////////

#include "CheckOut.h"

#ifdef TEST_CHECKOUT
int main()
{
	DbProvider dbp;
	DbCore<PayLoad> db = dbp.db();

	CheckOut chObj;
	std::cout << "\n\n----> Checkout particular file:";
	chObj.checkOutFile(db, "Executive", "Executive.h.1", "../Local/Checkout/");

	return 0;
}
#endif