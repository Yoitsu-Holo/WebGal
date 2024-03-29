`version 0.0.1 Alpha`

# 自研脚本功能文档

加载文件暂定名称为 elf（Executable Listing File）,文件形式为 `.elf` 后缀名

控制脚本暂定名称为 moe script（Multimedia Object Engine script）,文件形式为 `.moe` 后缀名

剧情脚本暂定名称为 miko script（multimedia Interaction Kernel Object script）,文件形式为 `.miko` 后缀名

`注`：文件后缀名只是一个简单的区分，具体文件内容的定义应该在ELF文件中书写，这意味着你可以使用`任何`扩展名，甚至没有扩展名来书写你的脚本代码，而只需保障其是一个`符合语法规则的文本文件`，其就是合法的脚本文件。

# ELF 主加载目标

全局只存在一个，在网页打开时最先被加载的文件，其包含以下内容：

| 名称 | 含义 |
|--|--|
| .file | 程序所有文件的名称、类型和url路径对应表 |
| .data | 全局变量部分，可以通过修饰符来定义常量或变量 |
| .start | 程序开始位置，相当于脚本的main函数位置 |
| .table | 程序所有函数与文件对应关系，程序不会处理，仅作为标识（后续可能作为预加载指示） |
| .form | 界面文件，通过json文件来注册，程序不会处理，仅作为标识（后续可能作为预加载指示） |
| .code | 代码文件，通过moe文件来注册，程序不会处理，仅作为标识（后续可能作为预加载指示） |

# Moe 脚本目标

该脚本仅用作剧本`流程控制`，不作为剧本使用，其应满足以下基本特点：

1. 流程化：可以方便简单的描述游戏代码逻辑
2. 结构化：可以方便的划分区块（函数），区块和区块之间可以互相调用
3. 本地化：可以方便的进行本地化（强制UTF-8编码），如设置全局变量来更改语言地区

# Miko 脚本目标

脚本用作剧本使用，本质是json文档，作用是存储剧本和本地化参数，并且可以内置Moe脚本，其满足以下几点

1. 流程化：可以方便简单的描述游戏代码逻辑
2. 结构化：可以方便的划分区块（函数），区块和区块之间可以互相调用
3. 本地化：可以方便的进行本地化（强制UTF-8编码），如设置全局变量来更改语言地区
