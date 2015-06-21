struct SONS_N_VARDEC {
	node *Init;
};

struct ATTRIBS_N_VARDEC {
	char *Id;
	char *Type;
};

struct SONUNION {
	struct SONS_N_VARDEC *N_vardec;
	..
};

struct ATTRIBUNION {
	struct ATTRIBS_N_VARDEC *N_vardec;
	..
};