node *n = ..;      /* retrieved elsewhere */
INFO *info = ..;   /* retrieved elsewhere */

TRAVpush(TR_mytrav);
n = TRAVdo(n, info);
TRAVpop();