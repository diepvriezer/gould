struct NODE {
	nodetype             nodetype;       /* type of node */
	int                  lineno;         /* line of definition */
	node*                error;          /* error node */
	struct SONUNION      sons;           /* child node structure */
	struct ATTRIBUNION   attribs;        /* attributes node structure */
};

typedef struct NODE node;