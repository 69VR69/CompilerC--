int main() {
	int a;
	a = 0;

	while (a < 3)
	{
		a = 1 + a;
		int b;
		b = 2;
		if (a > 2)
			break;
		while (b < 3)
		{
			b = 1 + b;
			int c;
			c = 1 + (2 * b) + a;
			if (b == 2)
				break;
		}
		if (a == 2)
			break;

		b = 1 + (2 * a);
	}
}
