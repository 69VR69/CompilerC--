int main() {
	int a;
	a = 2;

	while (a < 3)
	{
		a = 1 + a;
		int b;
		b = 2;
		while (b < 3)
		{
			b = 1 + b;
			int c;
			c = 1 + (2 * b) + a;
		}
	}
}
