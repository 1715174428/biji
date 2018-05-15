// This is main process of Electron, started as first thing when your
// app starts. This script is running through entire life of your application.
// It doesn't have any windows which you can see on screen, but we can open
// window from here.

import path from 'path';
import url from 'url';
import { app, Menu } from 'electron';
import { devMenuTemplate } from './menu/dev_menu_template';
import { editMenuTemplate } from './menu/edit_menu_template';
import {ipcMain} from 'electron';
import createWindow from './helpers/window';

// Special module holding environment variables which you declared
// in config/env_xxx.json file.
import env from './env';

const setApplicationMenu = () => {
  const menus = [editMenuTemplate];
  if (env.name !== 'production') {
    menus.push(devMenuTemplate);
  }
  Menu.setApplicationMenu(Menu.buildFromTemplate(menus));
};

// Save userData in separate folders for each environment.
// Thanks to this you can use production and development versions of the app
// on same machine like those are two separate apps.
if (env.name !== 'production') {
  const userDataPath = app.getPath('userData');
  app.setPath('userData', `${userDataPath} (${env.name})`);
}

app.on('ready', () => {
  setApplicationMenu();

  const mainWindow = createWindow('main', {
    width: 1000,
    height: 600,
  });

  
	mainWindow.loadURL("http://localhost:61279/Home/Login");
  if (env.name === 'development') {
    mainWindow.openDevTools();
  }
});

app.on('window-all-closed', () => {
  app.quit();
});

ipcMain.on('startTrayIconBlink',(event,arg)=>{
	startTrayIconBlink();
});
ipcMain.on('stopTrayIconBlink',(event,arg)=>{
	stopTrayIconBlink();
});

const electron = require('electron');
const Tray = electron.Tray;

const appIconPath = path.join(__dirname,'images/icon.ico');//图标路径，app\images下面
const emptyIconPath = path.join(__dirname,'images/empty.ico');//空白图标，托盘图标闪烁使用
var tray = null;
app.on('ready', function(){
  tray = new Tray(appIconPath);
});


function setTrayIcon(iconPath)
{
	try
	{
		tray.setImage(iconPath);
	}
	catch(err)//这是electron的bug，偶尔调用setImage会报异常，目前没有解决方案
	{
		console.error("设置图片出错,"+err.description);
	}	
}

//开始托盘图标闪烁
function startTrayIconBlink()
{
	if(tray.intervalId)//如果设置了tray.intervalId，说明还有在闪动的图标，不要多次执行闪动逻辑，否则会越闪越快
	{
		return;
	}
	var i=0;
	tray.intervalId=setInterval(()=>
	{
		i++;
		if(i%2==0)//偶数次数显示程序图标，奇数次数显示空白图标
		{
			setTrayIcon(appIconPath);
		}
		else
		{
			setTrayIcon(emptyIconPath);
		}
	},500);
}

//停止托盘图标闪烁
function stopTrayIconBlink()
{
	if(tray.intervalId)
	{
		clearInterval(tray.intervalId);
		tray.intervalId=null;//清空tray.intervalId
		setTrayIcon(appIconPath);//确保恢复到正常图标
	}
}



