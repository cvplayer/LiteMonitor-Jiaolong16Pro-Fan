# LiteMonitor 风扇转速插件

> 机械革命蛟龙 16 Pro（GM5HG7A）专用 · 任务栏实时显示 CPU/GPU 风扇转速

![效果](https://img.shields.io/badge/平台-Windows%2011-blue)  ![设备](https://img.shields.io/badge/设备-机械革命蛟龙16%20Pro-orange)  ![CPU](https://img.shields.io/badge/CPU-9955HX-red)

## 效果

LiteMonitor 任务栏实时显示：

```
CPU风扇 2560 RPM  │  GPU风扇 2670 RPM
```

颜色随转速变化：🟢 安静 → 🟡 负载 → 🔴 高转

## 特点

- **不需要控制中心**（CCU 关了也能用）
- **不需要驱动**（Windows 自带 ACPI 驱动）
- **不需要管理员权限**
- **开机自动后台运行**（无窗口）
- **即装即用**（复制一个 JSON + 双击一个 EXE）

## 快速开始

1. 下载 `ec-bridge.exe`，双击运行
2. 浏览器打开 `http://localhost:18900/fan`，看到数字说明成功
3. 把 `FanSpeed.json` 复制到 LiteMonitor 的 `resources/plugins/` 目录
4. LiteMonitor 设置 → 重载插件 → 启用「风扇转速」

[详细教程](使用教程-小白版.md)

## 原理

```
笔记本 EC 芯片
    │
    ▼
Windows ACPI 驱动 (\\.\ACPIDriver)
    │ DeviceIoControl
    ▼
ec-bridge.exe（HTTP :18900）
    │
    ▼
FanSpeed.json（LiteMonitor 插件）
    │
    ▼
任务栏显示 CPU/GPU 风扇 RPM
```

不依赖任何第三方软件，直接读取笔记本 EC（嵌入式控制器）寄存器。

## 文件说明

| 文件 | 用途 |
|:---|:---|
| `FanSpeed.json` | LiteMonitor 插件，放入 `resources/plugins/` |
| `ec-bridge/` | EC 直读桥接源码（.NET） |
| `ec-bridge.exe` | 编译好的独立程序（无需安装运行时） |
| `ec-scanner/` | EC 地址扫描工具（开发者用） |
| `start-bridge.vbs` | 开机静默启动脚本 |
| `使用教程-小白版.md` | 零基础图文教程 |
| `风扇插件开发文档.md` | 完整技术文档 |

## 适用设备

- ✅ 机械革命蛟龙 16 Pro / Yilong 15（GM5HG7A）
- ⚠️ 其他同方模具笔记本（需验证 EC 地址）

## 许可证

MIT
