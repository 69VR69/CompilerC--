int main() {
	int a;
	
	for (a = 0; a < 3; a = a + 1) {
		int b;
		for (b = 0; b < 3; b = b + 1) {
			int c;
			c = b + 8*a;
		}
		b = a + 1;
	}
}
