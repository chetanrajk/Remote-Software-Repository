/////////////////////////////////////////////////////////////////////////
// ServerPrototype.cpp - Console App that processes incoming messages  //
// ver 1.0                                                             //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018       //
/////////////////////////////////////////////////////////////////////////

#include "ServerPrototype.h"
#include "../FileSystem-Windows/FileSystemDemo/FileSystem.h"
#include <chrono>
#include "../SoftwareRepository/NoSqlDb (1)/DbCore/DbCore.h"
#include "../SoftwareRepository/RepositoryCore/RepositoryCore.h"

namespace MsgPassComm = MsgPassingCommunication;

using namespace Repository;
using namespace FileSystem;
using Msg = MsgPassingCommunication::Message;
using namespace NoSqlDb;

class DbProvider
{
public:
	DbCore<PayLoad>& db() { return db_; }
private:
	static DbCore<PayLoad> db_;
};

DbCore<PayLoad> DbProvider::db_;

//----< fuction for initializing db >---------------------------------------------

void initialDbA()
{
	DbProvider dbp;
	DbCore<PayLoad> db;
	PayLoad pl;
	DbElement<PayLoad> elem;
	elem.name("Comm.h.1");
	elem.descrip("John");
	pl.value() = "../Storage/Comm/Comm.h.1";
	pl.categories().push_back("Header File");
	pl.categories().push_back("Comm Feature");
	pl.status("Open");
	elem.payLoad(pl);
	elem.addChildKey("Message::Message.h.1");
	db["Comm::Comm.h.1"] = elem;
	pl.categories().clear(); elem.children().clear();

	elem.name("Message.h.1");
	elem.descrip("Parker");
	pl.value() = "../Storage/Message/Message.h.1";
	pl.categories().push_back("Header File");
	pl.categories().push_back("Message Feature");
	pl.status("Close");
	elem.payLoad(pl);
	db["Message::Message.h.1"] = elem;
	pl.categories().clear(); elem.children().clear();

	elem.name("Comm.cpp.1");
	elem.descrip("John");
	pl.value() = "../Storage/Comm/Comm.cpp.1";
	pl.categories().push_back("Source File");
	pl.categories().push_back("Comm Feature");
	pl.status("Close");
	elem.payLoad(pl);
	db["Comm::Comm.cpp.1"] = elem;
	pl.categories().clear(); elem.children().clear();

	elem.name("Comm.cpp.2");
	elem.descrip("John");
	pl.value() = "../Storage/Comm/Comm.cpp.2";
	pl.categories().push_back("Source File");
	pl.categories().push_back("Comm Feature");
	pl.status("Close");
	elem.payLoad(pl);
	db["Comm::Comm.cpp.2"] = elem;
	pl.categories().clear(); elem.children().clear();
	dbp.db() = db;
}

//----< fuction for initializing db >---------------------------------------------

void initialDbB()
{
	DbProvider dbp;
	DbCore<PayLoad> db;
	PayLoad pl;
	DbElement<PayLoad> elem;
	db = dbp.db();
	elem.name("Message.cpp.1");
	elem.descrip("Parker");
	pl.value() = "../Storage/Message/Message.cpp.1";
	pl.categories().push_back("Source File");
	pl.categories().push_back("Message Feature");
	pl.status("Close");
	elem.payLoad(pl);
	db["Message::Message.cpp.1"] = elem;
	pl.categories().clear(); elem.children().clear();

	elem.name("FileSystem.cpp.1");
	elem.descrip("Robert");
	pl.value() = "../Storage/FileSystem.cpp.1";
	pl.categories().push_back("Source File");
	pl.categories().push_back("Functionality");
	pl.status("Close");
	elem.payLoad(pl);
	elem.addChildKey("::Sockets.cpp.1");
	db["::FileSystem.cpp.1"] = elem;
	pl.categories().clear();
	elem.children().clear();

	elem.name("Sockets.cpp.1");
	elem.descrip("Justin");
	pl.value() = "../Storage/Sockets.cpp.1";
	pl.categories().push_back("Source File");
	pl.categories().push_back("Functionality");
	pl.status("Close");
	elem.payLoad(pl);
	db["::Sockets.cpp.1"] = elem;
	pl.categories().clear(); elem.children().clear();

	dbp.db() = db;
	showDb(db);
	//Utilities::putline();
	elem.payLoad().showDb(db);
}
//----< function to demonstrate getFiles>----------------

Files Server::getFiles(const Repository::SearchPath& path)
{
	return Directory::getFiles(path);
}

//----< function to demonstrate getDirs>----------------

Dirs Server::getDirs(const Repository::SearchPath& path)
{
	return Directory::getDirectories(path);
}

namespace MsgPassingCommunication
{
	std::string sendFilePath;
	std::string saveFilePath;

	//----< function to display message>----------------

	template<typename T>
	void show(const T& t, const std::string& msg)
	{
		std::cout << "\n  " << msg.c_str();
		for (auto item : t)
		{
			std::cout << "\n    " << item.c_str();
		}
	}

	//----<  test ServerProc simply echos message back to sender>----------------

	std::function<Msg(Msg)> echo = [](Msg msg) {
		Msg reply = msg;
		reply.to(msg.from());
		reply.from(msg.to());
		return reply;
	};

	//----< ServerProc for getFiles>----------------

	std::function<Msg(Msg)> getFiles = [](Msg msg) {
		Msg reply;
		reply.to(msg.from());
		reply.from(msg.to());
		reply.command("getFiles");
		std::string path = msg.value("path");
		if (path != "")
		{
			std::string searchPath = storageRoot;
			if (path != ".")
				searchPath = searchPath + "\\" + path;
			Files files = Server::getFiles(searchPath);
			size_t count = 0;
			for (auto item : files)
			{
				std::string countStr = Utilities::Converter<size_t>::toString(++count);
				reply.attribute("file" + countStr, item);
			}
		}
		else
		{
			std::cout << "\n  getFiles message did not define a path attribute";
		}
		return reply;
	};

	//----< ServerProc for getDirs>----------------

	std::function<Msg(Msg)> getDirs = [](Msg msg) {
		Msg reply;
		reply.to(msg.from());
		reply.from(msg.to());
		reply.command("getDirs");
		std::string path = msg.value("path");
		if (path != "")
		{
			std::string searchPath = storageRoot;
			if (path != ".")
				searchPath = searchPath + "\\" + path;
			Files dirs = Server::getDirs(searchPath);
			size_t count = 0;
			for (auto item : dirs)
			{
				if (item != ".." && item != ".")
				{
					std::string countStr = Utilities::Converter<size_t>::toString(++count);
					reply.attribute("dir" + countStr, item);
				}
			}
		}
		else
		{
			std::cout << "\n  getDirs message did not define a path attribute";
		}
		return reply;
	};

	//----< function to separate strings>----------------

	Keys getSeparated(std::string str)
	{
		Keys temp;
		std::string delim = "-";
		size_t pos = 0;
		while ((pos = str.find(delim)) != std::string::npos)
		{
			temp.push_back(str.substr(0, pos));
			str.erase(0, pos + delim.length());
		}
		return temp;
	}

	//----< ServerProc for checkInFile>----------------

	std::function<Msg(Msg)> checkInFile = [](Msg msg) {
		std::cout << "\n\nServer received checkInFile request:";
		Msg reply;
		reply.to(msg.from());
		reply.from(msg.to());
		reply.command("checkInFile");
		reply.attribute("verbose", "reply to checkIn");
		std::this_thread::sleep_for(std::chrono::milliseconds(1000));
		DbProvider dbp; CheckIn chkIn;
		DbCore<PayLoad> db = dbp.db();
		Keys dep, cat;;
		std::string pkg;
		std::string path = msg.value("path");
		pkg = path;
		pkg.erase(0, 1);
		dep = getSeparated(msg.dependency());
		cat = getSeparated(msg.category());
		std::string searchPath = storageRoot;
		if (path != "." && path != searchPath)
			searchPath = searchPath + "/" + path;     // preparing file path by combining storage root and reltive path received in msg 
		if (!FileSystem::Directory::exists(searchPath))
		{
			std::cout << "\n  file path does not exist";
			return reply;
		}
		std::string filePath = searchPath + "/" + msg.value("fileName");
		std::string fullSrcPath = saveFilePath;
		if (!FileSystem::Directory::exists(fullSrcPath))
		{
			std::cout << "\n  file destination path does not exist";
			return reply;
		}
		fullSrcPath += "/" + msg.value("fileName");
		chkIn.checkInFile(db, pkg, msg.fileName(), msg.author(), dep, fullSrcPath, filePath, cat, msg.status());
		dbp.db() = db;
		return reply;
	};

	//----< ServerProc for checkOutFile>----------------

	std::function<Msg(Msg)> checkOutFile = [](Msg msg) {
		std::cout << "\n\nServer received checkOutFile request:";
		Msg reply;
		reply.to(msg.from());
		reply.from(msg.to());
		reply.command("checkOutFile");
		reply.attribute("verbose", "reply to CheckOut");
		std::this_thread::sleep_for(std::chrono::milliseconds(1000));
		DbProvider dbp;
		DbCore<PayLoad> db = dbp.db();
		CheckOut chObj;	std::string pkg;
		std::string path = msg.value("path");
		pkg = path;	pkg.erase(0, 11);
		std::string file = msg.value("fileName");
		std::string newfilename = file.substr(0, file.length() - 2);
		reply.attribute("fileName", newfilename);
		reply.attribute("sendingFile", newfilename);
		if (path != "")
		{
			std::string searchPath = storageRoot;
			if (path != "." && path != searchPath)
				searchPath = searchPath + "\\" + path;     // preparing file path by combining storage root and reltive path received in msg 
			if (!FileSystem::Directory::exists(searchPath))
			{
				std::cout << "\n  file source path does not exist";
				return reply;
			}
			std::string filePath = searchPath + "/" + msg.value("fileName");
			std::string fullSrcPath = FileSystem::Path::getFullFileSpec(filePath);  // covert file path into full source path
			std::string fullDstPath = sendFilePath;
			if (!FileSystem::Directory::exists(fullDstPath))
			{
				std::cout << "\n  file destination path does not exist";
				return reply;
			}
			fullDstPath += "/" + msg.value("fileName");
			chObj.checkOutFile(db, pkg, msg.fileName(), fullSrcPath, fullDstPath);
		}
		else
		{
			std::cout << "\n  getDirs message did not define a path attribute";
		}
		return reply;
	};

	//----< ServerProc for browseFiles>----------------

	std::function<Msg(Msg)> browseFiles = [](Msg msg) {
		std::cout << "\n\nServer received browseFiles request";
		Msg reply;
		reply.to(msg.from());
		reply.from(msg.to());
		reply.command("browseFiles");
		reply.attribute("verbose", "sending files matching given criteria");
		std::this_thread::sleep_for(std::chrono::milliseconds(1000));

		DbProvider dbp;
		DbCore<PayLoad> db = dbp.db();
		Browse browseObj;
		Keys result;
		std::string resultFiles = "";
		std::string qFileName, qCategory, qDependency, qVersion, qParent;
		qFileName = msg.value("qFileName");
		qCategory = msg.value("qCategory");
		qDependency = msg.value("qDependency");
		qVersion = msg.value("qVersion");
		qParent = msg.value("qParent");

		result = browseObj.browseDb(db, qFileName, qCategory, qDependency, qVersion, qParent);

		for (Key str : result)
			resultFiles = resultFiles + str + "-";

		reply.attribute("resultFiles", resultFiles);

		return reply;
	};

	//----< ServerProc for connectToServer>----------------

	std::function<Msg(Msg)> connectToServer = [](Msg msg) {
		std::cout << "\n\nReceived connectToServer request";
		Msg reply;
		reply.to(msg.from());
		reply.from(msg.to());
		reply.command("connectToServer");
		reply.attribute("verbose", "Server is connected");
		std::this_thread::sleep_for(std::chrono::milliseconds(1000));
		return reply;
	};

	//----< ServerProc for doNotReply>----------------

	std::function<Msg(Msg)> doNotReply = [](Msg msg) {
		Msg reply;
		std::cout << "\n\nReceived doNotReply request";
		std::cout << "\n\nServer not sending any reply message";
		reply.command("doNotReply");
		std::this_thread::sleep_for(std::chrono::milliseconds(1000));
		return reply;
	};

	//----<  fuction for copying files to destination>----------

	bool copyFiles(std::string path, std::string filename, std::string newFilename)
	{
		std::string searchPath = storageRoot;
		if (path != "." && path != searchPath)
			searchPath = searchPath + "\\" + path;     // preparing file path by combining storage root and reltive path received in msg 
		if (!FileSystem::Directory::exists(searchPath))

		{
			std::cout << "\n  file source path does not exist";
			return false;
		}
		std::string filePath = searchPath + "/" + filename;
		std::string fullSrcPath = FileSystem::Path::getFullFileSpec(filePath);  // covert file path into full source path
		std::string fullDstPath = sendFilePath;
		if (!FileSystem::Directory::exists(fullDstPath))
		{
			std::cout << "\n  file destination path does not exist";
			return false;
		}
		fullDstPath += "/" + newFilename;
		FileSystem::File::copy(fullSrcPath, fullDstPath);
		return true;
	}

	//----< ServerProc for sendFile>----------------

	std::function<Msg(Msg)> fileText = [](Msg msg) {

		DbProvider dbp;	
		DbCore<PayLoad> db = dbp.db();	
		Msg reply;
		std::cout << "\n\nServer received fileText request";
		reply.to(msg.from());	
		reply.from(msg.to());
		reply.command("fileText");
		reply.attribute("verbose", "sending full file text and metadata");
		std::string path = msg.value("path");
		std::string file = msg.value("fileName");
		std::string newfilename = file.substr(0, file.length() - 2);
		reply.attribute("fileName", newfilename);
		reply.attribute("sendingFile", newfilename);
		std::string categories = "", dependencies = "";
		Query<PayLoad> q1(db); 
		Key tempkey; 
		Conditions<PayLoad> conds1;
		conds1.name(file);
		for (Key key : q1.select(conds1).keys())
		{
			tempkey = key;	break;
		}
		reply.attribute("datetime", db[tempkey].dateTime());
		reply.attribute("author", db[tempkey].descrip());
		reply.attribute("status", db[tempkey].payLoad().status());
		for (std::string str : db[tempkey].payLoad().categories())
			categories = categories + str + "-";
		reply.attribute("categories", categories);
		for (std::string str : db[tempkey].children())
			dependencies = dependencies + str + "-";
		reply.attribute("dependencies", dependencies);
		reply.attribute("version", file.substr(file.length() - 1, 1));
		if (path != "")
		{
			size_t pos = tempkey.find(":");
			std::string pkg = tempkey.substr(0, pos);
			bool done = copyFiles(pkg, file,newfilename);
		}
		else
		{
			std::cout << "\n  getDirs message did not define a path attribute";
		}
		std::this_thread::sleep_for(std::chrono::milliseconds(500));
		return reply;
	};


}

//----< main fuction for starting server prototype>----------

using namespace MsgPassingCommunication;

int main()
{
	SetConsoleTitleA("ServerPrototype console");

	sendFilePath = FileSystem::Directory::createOnPath("./SendFiles");
	saveFilePath = FileSystem::Directory::createOnPath("./SaveFiles");
	Server server(serverEndPoint, "ServerPrototype");
	MsgPassingCommunication::Context* pCtx = server.getContext();
	pCtx->saveFilePath = saveFilePath;
	pCtx->sendFilePath = sendFilePath;
	server.start();    // start server

	// add all serverProcs and corresponding commands
	server.addMsgProc("echo", echo);
	server.addMsgProc("getFiles", getFiles);
	server.addMsgProc("getDirs", getDirs);
	server.addMsgProc("serverQuit", echo);
	server.addMsgProc("checkInFile", checkInFile);
	server.addMsgProc("checkOutFile", checkOutFile);
	server.addMsgProc("browseFiles", browseFiles);
	server.addMsgProc("connectToServer", connectToServer);
	server.addMsgProc("fileText", fileText);
	server.addMsgProc("doNotReply", doNotReply);

	server.processMessages();

	Msg msg(serverEndPoint, serverEndPoint);  // send to self
	msg.name("msgToSelf");
	std::cout << "\n\n ======================================================";
	std::cout << "\n        Remote Code Repository - Server";
	std::cout << "\n ======================================================";
	std::cout << "\n";

	std::cout << "\n Initializing db ......\n";
	initialDbA();
	initialDbB();
	std::cin.get();
	std::cout << "\n";
	msg.command("serverQuit");
	server.postMessage(msg);
	server.stop();
	return 0;
}

