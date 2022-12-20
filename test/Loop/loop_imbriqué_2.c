int main() {
	int a;
	a = 1;

	do
	{
		a = 1 + a;
		int b;
		b = 2;
		do {
			b = 1 + b;
			int c;
			c = 2 * b + a;
		} while (b < 3);
		b = 1 + b;
	} while (a < 3);
}
