int malloc (int n) {
	int *p, r;
	p = 0;
	r = *p;
	*p = *p + n;
	return r;
}

int free (int p){}