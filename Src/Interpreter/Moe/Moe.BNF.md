```txt
BNF 语法
尖括号( < > )内包含的为必选项。
方括号( [ ] )内包含的为可选项。
大括号( { } )内包含的为可重复0至无数次的项。
竖线( | )表示在其左右两边任选一项，相当于"OR"的意思。
::= 是“被定义为”的意思。

注意，同一个语法可以使用写在多行的 "::=" 来描述，以增加描述的可读性

基本终结符可以是以下部分：
Type,       //^ 类型
Name,       //^ 名称
Number,     //^ 数字
String,     //^ 字符串
Keyword,    //* 关键字 (分散到 BNF 规则中)
Operator,   //^ 运算符
Delimiter,  //! 分隔符 (';'已被分析器简化)
CodeBlock,  //^ 代码块


syntax:

//* 可用语法规则 用户可以使用的语法规则，同时也是AST的生成规则
函数定义	::=	<函数标签> <函数名> <'('> <参数列表> <')'>

变量定义	::= <单变量定义> | <多变量定义>

算数表达式	::=	<算数表达式> <算数运算符> <算数表达式>
			::= <前算数运算符> <位运算表达式>
			::=	<'('> <算数表达式> <')'>
			::=	<变量> | <数字>

逻辑表达式	::= <逻辑表达式> <逻辑运算符> <逻辑表达式>
			::= <前缀逻辑运算符> <逻辑表达式>
			::=	<'('> <逻辑表达式> <')'>
			::=	<变量> | <数字>

函数调用	::= <变量名> <'('> <变量列表> <')'>

赋值表达式	::= <变量> <'='> <表达式>
			::= <变量> <'='> <逻辑表达式> // 真为 1、1.0、"true"，假为 0、0.0、"false"
			::= <变量> <'='> <位运算表达式>
			::= <变量> <'='> <函数调用>
			::= <变量> <'='> <字符串>

条件分支	::=	<'if'> <'('> <逻辑表达式> <')'> <程序>
				{ <'elif'> <'('> <逻辑表达式> <')'> <程序> }
				[ <'else'> <程序> ]

循环分支	::= <'while'> <'('> <逻辑表达式> <')'> <程序>

程序		::= { < 算数表达式 | 逻辑表达式 | 赋值表达式 > }
			::= { < 变量定义 > }
			::= { < 流程 | 函数调用 > }
			::= < NULL >		// 空程序

//* 内部隐藏规则 用户无法使用，用于语法中间处理
函数标签	::= < 'func' > <基础类型>
变量类型	::=	< 'const' | 'static' | 'var' > <基础类型>
参数列表	::= <单变量定义> { <','> <单变量定义> }
变量列表	::= <变量信息> [ ',' <变量信息> ]
变量信息	::= <名称> [ <'['> <整数> <']'> ]
单变量定义	::= <变量类型> <变量信息>
多变量定义	::=	<单变量定义> { <','> <变量信息> }

//* 基本映射规则 映射到基本ntoken类型
变量名	::= <Name>
函数名	::= <Name>
标签名	::= <Name>
整数	::= <Number>
浮点数	::=	<Number> <(Delimiter) '.'> <Number>
基础类型::= <Type>
算数运算符	::= <(Operator) '^^' >				// 乘方			（第一优先级）
			::= <(Operator) '*' | '/' | '%' >	// 乘除，取余	（第二优先级）
			::= <(Operator) '+' | '-' >			// 加减			（第三优先级）
			::= <(Operator) '&' | '|' | '^' | '<<' | '>>' >
			::= <(Operator) '~' >

逻辑运算符	::= <(Operator) '==' | '!=' | '>' | '<' | '>=' | '<=' | '&&' | '||'>
前逻辑运算符::= <(Operator) '!' >
```