int main() {
	int a;
	
	for (a = 0; a < 3; a = a + 1) {
		int b;
		if (a == 2)
			continue;
		for (b = 0; b < 3; b = b + 1) {
			int c;
			c = b + 8 * a;
			if (b == 1) {
				continue;
			}
		}

		if (a == 4)
			break;
		b = a + 1;
	}
}
