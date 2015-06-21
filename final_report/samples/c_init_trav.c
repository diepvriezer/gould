{
	node *n, info *info;	/* retrieved elsewhere */
	
	TRAVpush(TR_mytrav);	/* push traversal ID */
	n = TRAVdo(n, info);	/* initiate */
	TRAVpop();			  /* pop traversal */
	
	return n;				/* return control */
}