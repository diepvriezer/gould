node *TRAVsons (node *arg_node, info *arg_info)
{
	switch (NODE_TYPE (arg_node)) {
		case N_program:
		TRAV (PROGRAM_DECLARATIONS (arg_node), arg_info);
		break;
		
		case N_declarations:
		TRAV (DECLARATIONS_DECLARATION (arg_node), arg_info);
		...
}