#include <iostream>
#include <memory>
#include "http_server.h"
#include <cstring>
#include <string>
#include <tchar.h>
using namespace std;
// ��ʼ��HttpServer��̬���Ա
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
	//���������ַ���
	int index = body.find('=');
	string value = body.substr(index+1, body.length()-1);
	std::cout << value << endl;
	string axis = value.substr(0,1);
	string stopOperation =  string(axis);
	stopOperation.append("ͣ");
	char* operation = (char*)value.data();
	
	std::cout << "axis: " << axis << " operation: " << operation << " stop :" << stopOperation <<endl;

	//����Ϊ�ٿ�windows�������
	const char* param = "WindowsForms10.Window.8.app.0.33c0d9d";
	//��һ��������Windows����Ĵ����࣬�ڶ��������Ǵ���ı��⡣����Ϥwindows��̵���Ҫ����һЩWindows�������ݽṹ��֪ʶ������������windows��Ϣѭ�����������Ķ������ÿ�̫�ࡣ
	HWND main = FindWindow(NULL, _T("yj"));
	HWND pDSubWnd = ::GetDlgItem(main, 0x0002);
	HWND pDSsubWnd = FindWindowEx(pDSubWnd, NULL, param, "ʾ��");
	HWND pDSssubWnd = ::GetDlgItem(pDSsubWnd, 0x0003);
	HWND pDSsssubWnd = FindWindowEx(pDSssubWnd, NULL, param, "ֱ��");
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
	HWND pDSsubWnd = FindWindowEx(pDSubWnd, NULL, "WindowsForms10.Window.8.app.0.378734a", "���");
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