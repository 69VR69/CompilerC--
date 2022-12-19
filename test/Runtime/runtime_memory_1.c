int main() {
	int *p;
	p = malloc(10);
	*p = 10;
	*(p+7) = 20;
	free(p);
}