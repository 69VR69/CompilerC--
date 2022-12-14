int main() {
	int a;
	a = 0;

	do
	{
		a = 1 + a;
		int b;
		b = 2;
		do {
			b = 1 + b;
			int c;
			c = 2 * b + a;
		} while (b < 10);
		b = 1 + b;
	} while (a < 10);
}
