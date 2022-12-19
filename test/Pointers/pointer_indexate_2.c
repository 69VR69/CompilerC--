int test(int[] a, int b) {
	int[] c;
	c[0] = a[0] + b;
	c[1] = a[1] + b;
	return c;
}

int main() {
	int[] a;
	a[0] = 1;
	a[1] = 2;
	int[] c;
	c = test(a, 2);
	return c[0];
}