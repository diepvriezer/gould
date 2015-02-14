grammar Civic;

/*
 * Parser Rules
 */
prog		:	stat+
			;

stat		:	ID '=' expr ';'												# Assign
			;
			

expr		:	'(' expr ')'												# Parens
		
			|	expr op=(MUL|DIV|MOD) expr									# Binop
			|	expr op=(ADD|SUB) expr										# Binop

			|	ID															# Var
			|	constant													# Const
			;
exprList	:	expr (',' expr)*		;


type		:	'float' | 'int' | 'bool' | 'void';
constant	:	'true'														# BoolConst
			|	 'false'													# BoolConst
			|	 INT														# IntConst
			|	 FLOAT														# FloatConst
			;




/*
 * Lexer Rules
 */
COMMENT
			:	( '//' ~[\r\n]* '\r'? '\n'
			|	'/*' .*? '*/'
			)			-> channel(HIDDEN)
			;

ADD	:	'+';
SUB	:	'-';
MUL	:	'*';
DIV	:	'/';
MOD	:	'%';

FLOAT		:	[0-9]+ '.' [0-9]+;
INT			:	[0-9]+;
ID			:	[a-zA-Z_]([a-zA-Z_]|[0-9])*;

WS			:	[ \r\t\n]+ -> channel(HIDDEN);