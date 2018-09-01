#pragma once
///////////////////////////////////////////////////////////////////////
// ServerPrototype.h - Console App that processes incoming messages  //
// ver 1.1                                                           //
// Chetanraj Kadam, CSE687 - Object Oriented Design, Spring 2018     //
///////////////////////////////////////////////////////////////////////
/*
*  Package Operations:
* ---------------------
*  Package contains one class, Server, that contains a Message-Passing Communication
*  facility. It processes each message by invoking an installed callable object
*  defined by the message's command key.
*  - This is implemented with a message dispatcher (unodered_map<Msg.Id,ServerProc>
*    where ServerProcs are defined for each type of processing required by the server.
*
*  Message handling runs on a child thread, so the Server main thread is free to do
*  any necessary background processing (none, so far).
*
*  Required Files:
* -----------------
*  ServerPrototype.h, ServerPrototype.cpp
*  Comm.h, Comm.cpp, IComm.h
*  Message.h, Message.cpp (static library)
*  Process.h, Process.cpp (static library)
*  FileSystem.h, FileSystem.cpp
*  Utilities.h
*
*  Maintenance History:
* ----------------------
*  ver 1.1 : 04/09/2018
*  - added ServerProcs for
*    - sending files for popup display
*    - executing remote analysis
*  ver 1.0 : 03/27/2018
*  - first release
*/
#include <vector>
#include <string>
#include <unordered_map>
#include <functional>
#include <thread>
#include "../CppCommWithFileXfer/Message/Message.h"
#include "../CppCommWithFileXfer/MsgPassingComm/Comm.h"
#include "../SoftwareRepository/NoSqlDb (1)/DbCore/DbCore.h"
#include "../SoftwareRepository/Payload/PayLoad.h"
#include <windows.h>
#include <tchar.h>

namespace Repository
{
	using File = std::string;
	using Files = std::vector<File>;
	using Dir = std::string;
	using Dirs = std::vector<Dir>;
	using SearchPath = std::string;
	using Key = std::string;
	using Msg = MsgPassingCommunication::Message;
	using ServerProc = std::function<Msg(Msg)>;
	using MsgDispatcher = std::unordered_map<Key, ServerProc>;

	const SearchPath storageRoot = "../Storage";  // root for all server file storage
	const MsgPassingCommunication::EndPoint serverEndPoint("localhost", 8080);  // listening endpoint




	class Server
	{
	public:
		Server(MsgPassingCommunication::EndPoint ep, const std::string& name);
		void start();
		void stop();
		void addMsgProc(Key key, ServerProc proc);
		bool hasMessageKey(const Key& key);
		void processMessages();
		void postMessage(MsgPassingCommunication::Message msg);
		MsgPassingCommunication::Message getMessage();
		static Dirs getDirs(const SearchPath& path = storageRoot);
		static Files getFiles(const SearchPath& path = storageRoot);
		MsgPassingCommunication::Context* getContext();
		std::string setSendFilePath(const std::string& relPath);
		std::string setSaveFilePath(const std::string& relPath);
		std::string getSendFilePath();
		std::string getSaveFilePath();
		//void initializeDb();
	private:
		MsgPassingCommunication::Comm comm_;
		MsgDispatcher dispatcher_;
		std::thread msgProcThrd_;
		NoSqlDb::DbCore<NoSqlDb::PayLoad> db_;
	};
	//----< return reference to MsgPassingCommunication context >--------

	inline MsgPassingCommunication::Context* Server::getContext()
	{
		return comm_.getContext();
	}
	//----< initialize server endpoint and give server a name >----------

	inline Server::Server(MsgPassingCommunication::EndPoint ep, const std::string& name)
		: comm_(ep, name) {
		//initialDb();
	}

	//----< fuction for setting sendFile path>----------

	inline std::string Server::setSendFilePath(const std::string& relPath)
	{
		comm_.setSendFilePath(relPath);
	}

	//----< fuction for setting saveFile path>----------

	inline std::string Server::setSaveFilePath(const std::string& relPath)
	{
		comm_.setSaveFilePath(relPath);
	}

	//----< fuction for getting sendFile path>----------

	inline std::string Server::getSendFilePath()
	{
		comm_.getSendFilePath();
	}
	//----< fuction for getting saveFile path>----------

	inline std::string Server::getSaveFilePath()
	{
		comm_.getSaveFilePath();
	}
	//----< start server's instance of Comm >----------------------------

	inline void Server::start()
	{
		comm_.start();
	}
	//----< stop Comm instance >-----------------------------------------

	inline void Server::stop()
	{
		if (msgProcThrd_.joinable())
			msgProcThrd_.join();
		comm_.stop();
	}
	//----< pass message to Comm for sending >---------------------------

	inline void Server::postMessage(MsgPassingCommunication::Message msg)
	{
		comm_.postMessage(msg);
	}
	//----< get message from Comm >--------------------------------------

	inline MsgPassingCommunication::Message Server::getMessage()
	{
		Msg msg = comm_.getMessage();
		return msg;
	}
	//----< add ServerProc callable object to server's dispatcher >------

	inline void Server::addMsgProc(Key key, ServerProc proc)
	{
		dispatcher_[key] = proc;
	}
	//----< does server have specified key? >----------------------------

	inline bool Server::hasMessageKey(const Key& key)
	{
		for (auto item : dispatcher_)
		{
			if (item.first == key)
				return true;
		}
		return false;
	}
	//----< start processing messages on child thread >------------------

	inline void Server::processMessages()
	{
		auto proc = [&]()
		{
			if (dispatcher_.size() == 0)
			{
				std::cout << "\n  no server procs to call";
				return;
			}
			while (true)
			{
				try {
					Msg msg = getMessage();   // call getMessage() for receiving messages
					std::cout << "\n\n---> Server Received message: " << msg.command() << " from " << msg.from().toString();
					if (msg.containsKey("verbose"))
					{
						//std::cout << "\n";
						msg.show();
					}
					if (msg.command() == "serverQuit")
						break;
					Msg reply;
					reply.to(msg.from()); reply.from(msg.to());
					if (hasMessageKey(msg.command()))
					{
						reply = dispatcher_[msg.command()](msg);
					}
					else
					{
						reply.command("error - unknown command");
					}
					if (msg.to().port != msg.from().port && msg.command() != "doNotReply")  // avoid infinite message loop
					{
						std::cout << "\n\nServer sending response to " << reply.command() << " request";
						if (reply.containsKey("verbose"))
							reply.show();
						postMessage(reply);
					}
					else
						std::cout << "\n";
				}
				catch (const std::exception& e) {
					std::cout << "Something wrong happened. details: " << e.what();
				}
			}
			std::cout << "\n  server message processing thread is shutting down";
		};
		std::thread t(proc);
		msgProcThrd_ = std::move(t);
	}
}