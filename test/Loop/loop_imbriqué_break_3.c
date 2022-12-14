int main() {
	int a;
	
	for (a = 0; a < 10; a = a + 1) {
		int b;
		if (a > 8)
			break;
		for (b = 0; b < 10; b = b + 1) {
			int c;
			c = b + 8 * a;
			if(b==5)
				break;
		}
		if (a == 4)
			break;
		b = a + 1;
	}
}
