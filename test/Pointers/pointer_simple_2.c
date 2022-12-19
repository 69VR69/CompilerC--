int test(int* a, int b) {
	int* c;
	c = *a + b;
	return &c;
}

int main() {
	int a;
	a = 1;
	int c;
	c = test(&a, 2);
}