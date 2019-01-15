#include <iostream>
#include <memory>
#include "http_server.h"
#include <cstring>
#include <string>
#include <tchar.h>
#include <set>
#include <map>


using namespace std;
// 初始化HttpServer静态类成员
mg_serve_http_opts HttpServer::s_server_option;
std::string HttpServer::s_web_dir = "./web";
std::unordered_map<std::string, ReqHandler> HttpServer::s_handler_map;
std::unordered_set<mg_connection *> HttpServer::s_websocket_session_set;



//机械臂参数
set<string> stringsSet;
map<string,int> operationHandleMap;
const string operationStrings[] = {"示教","直角","x+","x-","x停","y+","y-","y停" ,"z+","z-","z停" };

bool handle_fun1(std::string url, std::string body, mg_connection *c, OnRspCallback rsp_callback)
{
	// do sth
	std::cout << "handle fun1" << std::endl;
	std::cout << "url: " << url << std::endl;
	std::cout << "body: " << body << std::endl;

	rsp_callback(c, "rsp1");

	return true;
}

bool handle_fun2(std::string url, std::string body, mg_connection *c, OnRspCallback rsp_callback)
{
	// do sth
	std::cout << "handle fun2" << std::endl;
	std::cout << "url: " << url << std::endl;
	std::cout << "body: " << body << std::endl;

	rsp_callback(c, "rsp2");

	return true;
}



void sendMessageToMachine(string body) {
	//解析请求字符串
	int index = body.find('=');
	string value = body.substr(index+1, body.length()-1);
	string axis = value.substr(0, 1);
	string operationStr = value.substr(1);
	if (operationStr.compare("%2B") == 0){
		value.replace(1, 3, "+");
	}
	string stop = string(axis);
	stop.append("停");
	cout << "value = " << value << " stop = " << stop << " handle hwnd = " << operationHandleMap[value] << " stop hwnd = "<< operationHandleMap[stop]<< endl;
	//cout << " shijiao = " << operationHandleMap[operationStrings[0]] << " zhijiao = " << operationHandleMap[operationStrings[1]] << endl;
	HWND yj = FindWindow(NULL, "yj");
	//Sleep(500);
	//下载
	HWND firstPanel = ::GetDlgItem(yj, 0x00000002);
	HWND shijiao = ::GetDlgItem(firstPanel, operationHandleMap[operationStrings[0]]);
	HWND secondPanel = ::GetDlgItem(shijiao, 0x00000003);
	HWND zhijiao = ::GetDlgItem(secondPanel, operationHandleMap[operationStrings[1]]);
	HWND operation = ::GetDlgItem(zhijiao, operationHandleMap[value.c_str()]);
	SendMessage(operation, BM_CLICK, 0, 0);
	Sleep(200);
	HWND stopOperation = ::GetDlgItem(zhijiao, operationHandleMap[stop.c_str()]);
	SendMessage(stopOperation, BM_CLICK, 0, 0);
	return;
}

bool handleRoboticArmMovement(std::string url, std::string body, mg_connection *c, OnRspCallback rsp_callback) {
	// do sth
	std::cout << "handle operation" << std::endl;
	std::cout << "url: " << url << std::endl;
	std::cout << "body: " << body << std::endl;
	sendMessageToMachine(body);
	rsp_callback(c, "success");
	return true;
}

int Pnum = 0, Cnum;//父窗口数量，每一级父窗口的子窗口数量  

//---------------------------------------------------------  
//EnumChildWindows回调函数，hwnd为指定的父窗口  
//---------------------------------------------------------  
BOOL CALLBACK EnumChildWindowsProc(HWND hWnd, LPARAM lParam)
{
	char WindowTitle[100] = { 0 };
	Cnum++;
	::GetWindowText(hWnd, WindowTitle, 100);
	if (stringsSet.find(WindowTitle) != stringsSet.end()){
		printf("%s\n", WindowTitle);
		operationHandleMap[WindowTitle] = (int)hWnd;
		cout << "hWnd = " << hWnd << endl;
	}
	return true;
}
//---------------------------------------------------------  
//EnumWindows回调函数，hwnd为发现的顶层窗口  
//---------------------------------------------------------  
BOOL CALLBACK EnumWindowsProc(HWND hWnd, LPARAM lParam)
{
	if (GetParent(hWnd) == NULL && IsWindowVisible(hWnd))  //判断是否顶层窗口并且可见  
	{
		Pnum++;
		Cnum = 0;
		char WindowTitle[100] = { 0 };
		::GetWindowText(hWnd, WindowTitle, 100);
		/*printf("-------------------------------------------\n");
		printf("%d: %s\n", Pnum, WindowTitle);*/
		string targetParentTitle = "yj";
		if (strcmp(WindowTitle, targetParentTitle.c_str()) == 0) {
			cout << "Find Main Window yj" << endl;
			EnumChildWindows(hWnd, EnumChildWindowsProc, NULL); //获取父窗口的所有子窗口  
		}
	}
	return true;
}


bool getHandle() {
	for (int i = 0; i < 11; i++) {
		stringsSet.insert(operationStrings[i]);
	}
	//获取屏幕上所有的顶层窗口,每发现一个窗口就调用回调函数一次  
	EnumWindows(EnumWindowsProc, NULL);
	if (operationHandleMap.empty()){
		cout << "get handle error" << endl;
		return false;
	}
	else {
		cout << "get handle success" << endl;
		return true;
	}
}

int main(int argc, char *argv[]) 
{
	std::string port = "7999";
	auto http_server = std::shared_ptr<HttpServer>(new HttpServer);
	if (!getHandle()){
		return 0;
	}
	http_server->Init(port);
	//start drill
	//startDrill();
	// add handler
	http_server->AddHandler("/api/fun1", handle_fun1);
	http_server->AddHandler("/api/fun2", handle_fun2);
	http_server->AddHandler("/api/movement", handleRoboticArmMovement);

	http_server->Start();
	

	return 0;
}