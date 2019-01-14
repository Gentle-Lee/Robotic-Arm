#include <iostream>
#include <memory>
#include "http_server.h"
#include <cstring>
#include <string>
#include <tchar.h>
using namespace std;
// 初始化HttpServer静态类成员
mg_serve_http_opts HttpServer::s_server_option;
std::string HttpServer::s_web_dir = "./web";
std::unordered_map<std::string, ReqHandler> HttpServer::s_handler_map;
std::unordered_set<mg_connection *> HttpServer::s_websocket_session_set;

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
	std::cout << value << endl;
	string axis = value.substr(0,1);
	string stopOperation =  string(axis);
	stopOperation.append("停");
	char* operation = (char*)value.data();
	
	std::cout << "axis: " << axis << " operation: " << operation << " stop :" << stopOperation <<endl;

	//以下为操控windows窗体程序
	const char* param = "WindowsForms10.Window.8.app.0.33c0d9d";
	//第一个参数是Windows窗体的窗体类，第二个参数是窗体的标题。不熟悉windows编程的需要先找一些Windows窗体数据结构的知识来看看，还有windows消息循环处理，其他的东西不用看太多。
	HWND main = FindWindow(NULL, _T("yj"));
	HWND pDSubWnd = ::GetDlgItem(main, 0x0002);
	HWND pDSsubWnd = FindWindowEx(pDSubWnd, NULL, param, "示教");
	HWND pDSssubWnd = ::GetDlgItem(pDSsubWnd, 0x0003);
	HWND pDSsssubWnd = FindWindowEx(pDSssubWnd, NULL, param, "直角");
	HWND pDSssssubWnd = FindWindowEx(pDSsssubWnd, NULL, param, operation);
	SendMessage(pDSssssubWnd, BM_CLICK, 0, 0);
	Sleep(200);
	HWND pDSsssssubWnd = FindWindowEx(pDSsssubWnd, NULL, param, stopOperation.c_str());
	SendMessage(pDSsssssubWnd, BM_CLICK, 0, 0);
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

void startDrill() {
	HWND main = FindWindow(NULL, _T("yj"));
	HWND pDSubWnd = ::GetDlgItem(main, 0x0002);
	HWND pDSsubWnd = FindWindowEx(pDSubWnd, NULL, "WindowsForms10.Window.8.app.0.378734a", "输出");
	ShowWindow(pDSsubWnd, 9);
	HWND pDSsssubWnd = FindWindowEx(pDSsubWnd, NULL, "WindowsForms10.BUTTON.app.0.378734a", "o08");
	SendMessage(pDSsssubWnd, BM_CLICK, 0, 0);
	std::cout << "drill start" << endl;
}

int main(int argc, char *argv[]) 
{
	std::string port = "7999";
	auto http_server = std::shared_ptr<HttpServer>(new HttpServer);
	http_server->Init(port);
	//start drill
	startDrill();
	// add handler
	http_server->AddHandler("/api/fun1", handle_fun1);
	http_server->AddHandler("/api/fun2", handle_fun2);
	http_server->AddHandler("/api/movement", handleRoboticArmMovement);
	http_server->Start();
	

	return 0;
}