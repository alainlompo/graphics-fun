package org.lompo.technologies.visualworks.labs.yawa;

import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import javax.imageio.ImageIO;

public class Texture {
	public int[] pixels;
	private String loc;
	public final int SIZE;
	
	public Texture(String location, int size) {
		loc = location;
		SIZE = size;
		pixels = new int[SIZE * SIZE];
		load();
	}
	
	private File getFile(String resourceFolderBoundFilePath) {
		ClassLoader classLoader = getClass().getClassLoader();
		return new File(classLoader.getResource(resourceFolderBoundFilePath).getFile());
	}
	
	private void load() {
		try {
			BufferedImage image = ImageIO.read(getFile(loc));
			int w = image.getWidth();
			int h = image.getHeight();
			image.getRGB(0, 0, w, h, pixels, 0, w);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	public static Texture wood = new Texture("textures/wood_wall_64_1.jpg", 64);
	public static Texture brick = new Texture("textures/wood_wall_64_2.jpg", 64);
	public static Texture bluestone = new Texture("textures/wood_wall_64_3.jpg", 64);
	public static Texture stone = new Texture("textures/wood_wall_64_4.jpg", 64);
}
