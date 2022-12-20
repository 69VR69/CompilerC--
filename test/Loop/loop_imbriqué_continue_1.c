int main() {
	int a;
	a = 0;

	while (a < 3)
	{
		a = 1 + a;
		int b;
		b = 2;
		if (a == 2)
			continue;
		while (b < 3)
		{
			b = 1 + b;
			if (b == 2)
				continue;
			int c;
			c = 1 + (2 * b) + a;
		}

		if (a == 2)
			continue;
		b = 1 + (2 * a);
	}
}
