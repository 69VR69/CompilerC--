﻿int main() {
	int a;
	a = 0;

	do
	{
		a = 1 + a;
		int b;
		b = 2;
		if (a == 2)
			continue;
		do {
			b = 1 + b;
			if(b==2)
				continue;
			int c;
			c = 2 * b + a;
		} while (b < 3);
		if (a == 2)
			continue;
		b = 1 + (2 * a);
	} while (a < 3);
}
