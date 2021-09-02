using System.Collections.Generic;
using System.Linq;
using System.IO;
using DavidSosvald_MichalTopfer;
using JosefPelikan;

Debug.Assert(scene != null);
Debug.Assert(scene is ITimeDependent);
Debug.Assert(context != null);


//////////////////////////////////////////////////
// CSG scene

Animator a; // 'a' is used to register params (names, parsers, interpolators) during scene creation
if (context.ContainsKey("animator")) {
  scene.Animator = (ITimeDependent)((Animator)context["animator"]).Clone();
  a = null; // params were already registered when Animator was created (scene is the same)
} else {
  string keyframes_file = Path.Combine(Path.GetDirectoryName((string)context[PropertyName.CTX_SCRIPT_PATH]), "RubikAnimation.yaml");
  a = new Animator(keyframes_file);
  scene.Animator = a;
  context["animator"] = a;
}

AnimatedCSGInnerNode root = new AnimatedCSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] { 1.0, 0.6, 0.1 }, 0.1, 0.8, 0.2, 16));
scene.Intersectable = root;

// Background:
scene.BackgroundColor = new double[] { 0.001, 0.001, 0.001 };
scene.Background = new StarBackground(scene.BackgroundColor, 600, 0.004, 0.5, 1.6, 1.0);
//scene.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };
//scene.Background = new DefaultBackground(scene.BackgroundColor);

// Light sources:
scene.Sources = new LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 3.0, -3.0), 1.0));
scene.Sources.Add(new PointLightSource(new Vector3d(5.0, 3.0, -3.0), 1.0));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, -3.0, -3.0), 1.0));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 3.0, 3.0), 1.0));

// Camera:
scene.Camera = new KeyframesAnimatedStaticCamera(a);

// --- NODE DEFINITIONS ----------------------------------------------------

// Params dictionary:
Dictionary<string, string> p = Util.ParseKeyValueList(param);

// materials
double n = 1.6;
Util.TryParse(p, "n", ref n);
PhongMaterial bm = new PhongMaterial(new double[] { 0.1, 0.1, 0.1 }, 0.0, 0.03, 0.01, 128);
bm = new PhongMaterial(new double[] { 0.1, 0.1, 0.1 }, 0.03, 0.03, 0.08, 128);
bm.n  = n;
bm.Kt = 0.9;
PhongMaterial white = new PhongMaterial(new double[] { 1, 1, 1 }, 0.0, 0.03, 0.01, 128);
PhongMaterial orange = new PhongMaterial(new double[] { 1, 0.4, 0.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial blue = new PhongMaterial(new double[] { 0.0, 0.0, 1.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial red = new PhongMaterial(new double[] { 1.0, 0.0, 0.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial green = new PhongMaterial(new double[] { 0.0, 1.0, 0.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial yellow = new PhongMaterial(new double[] { 1.0, 1.0, 0.0 }, 0.0, 0.03, 0.01, 128);

PhongMaterial color1 = new PhongMaterial(new double[] { 0.0, 0.0, 1.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color2 = new PhongMaterial(new double[] { 1.0, 0.0, 0.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color3 = new PhongMaterial(new double[] { 0.0, 1.0, 0.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color4 = new PhongMaterial(new double[] { 0.5, 0.0, 1.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color5 = new PhongMaterial(new double[] { 0.0, 0.5, 1.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color6 = new PhongMaterial(new double[] { 1.0, 0.5, 0.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color7 = new PhongMaterial(new double[] { 1.0, 0.0, 0.5 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color8 = new PhongMaterial(new double[] { 0.5, 1.0, 0.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color9 = new PhongMaterial(new double[] { 0.0, 1.0, 0.5 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color10 = new PhongMaterial(new double[] { 0.5, 0.5, 0.0 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color11 = new PhongMaterial(new double[] { 0.0, 0.5, 0.5 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color12 = new PhongMaterial(new double[] { 0.5, 0.0, 0.5 }, 0.0, 0.03, 0.01, 128);
PhongMaterial color13 = new PhongMaterial(new double[] { 0.5, 0.5, 0.5 }, 0.0, 0.03, 0.01, 128);

// Cubes
Cube c;
Cube s;
AnimatedNodeTransform ac;

#region Centers
#region White center
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "W", null, new Vector3d(-0.5, 0.6, -0.5)); // W
ac.InsertChild(c, Matrix4d.Identity);
// white sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, white);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.12, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Blue center
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "B", null, new Vector3d(-1.6, -0.5, -0.5)); // B
ac.InsertChild(c, Matrix4d.Identity);
// blue sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, blue);
ac.InsertChild(s, Matrix4d.CreateTranslation(-0.02, 0.05, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Red center
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "R", null, new Vector3d(-0.5, -0.5, 0.6)); // R
ac.InsertChild(c, Matrix4d.Identity);
// red sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, red);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, 0.12) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Yellow center
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "Y", null, new Vector3d(-0.5, -1.6, -0.5)); // Y
ac.InsertChild(c, Matrix4d.Identity);
// yellow sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, yellow);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, -0.02, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Orange center
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "O", null, new Vector3d(-0.5, -0.5, -1.6)); // O
ac.InsertChild(c, Matrix4d.Identity);
// orange sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, orange);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, -0.02) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Green center
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "G", null, new Vector3d(0.6, -0.5, -0.5)); // G
ac.InsertChild(c, Matrix4d.Identity);
// green sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, green);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.12, 0.05, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#endregion

#region Edges
#region Blue-white edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "BW", null, new Vector3d(-1.6, 0.6, -0.5)); // BW
ac.InsertChild(c, Matrix4d.Identity);
// white sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, white);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.12, 0.05) * Matrix4d.Scale(0.9));
// blue sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, blue);
ac.InsertChild(s, Matrix4d.CreateTranslation(-0.02, 0.05, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Red-white edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "RW", null, new Vector3d(-0.5, 0.6, 0.6)); // RW
ac.InsertChild(c, Matrix4d.Identity);
// white sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, white);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.12, 0.05) * Matrix4d.Scale(0.9));
// red sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, red);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, 0.12) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Green-white edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "GW", null, new Vector3d(0.6, 0.6, -0.5)); // GW
ac.InsertChild(c, Matrix4d.Identity);
// white sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, white);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.12, 0.05) * Matrix4d.Scale(0.9));
// green sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, green);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.12, 0.05, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Orange-white edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "OW", null, new Vector3d(-0.5, 0.6, -1.6)); // OW
ac.InsertChild(c, Matrix4d.Identity);
// white sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, white);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.12, 0.05) * Matrix4d.Scale(0.9));
// orange sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, orange);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, -0.02) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Blue-red edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "BR", null, new Vector3d(-1.6, -0.5, 0.6)); // BR
ac.InsertChild(c, Matrix4d.Identity);
// blue sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, blue);
ac.InsertChild(s, Matrix4d.CreateTranslation(-0.02, 0.05, 0.05) * Matrix4d.Scale(0.9));
// red sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, red);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, 0.12) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Green-red edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "GR", null, new Vector3d(0.6, -0.5, 0.6)); // GR
ac.InsertChild(c, Matrix4d.Identity);
// red sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, red);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, 0.12) * Matrix4d.Scale(0.9));
// green sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, green);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.12, 0.05, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Green-orange edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "GO", null, new Vector3d(0.6, -0.5, -1.6)); // GO
ac.InsertChild(c, Matrix4d.Identity);
// orange sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, orange);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, -0.02) * Matrix4d.Scale(0.9));
// green sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, green);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.12, 0.05, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Blue-orange edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "BO", null, new Vector3d(-1.6, -0.5, -1.6)); // BO
ac.InsertChild(c, Matrix4d.Identity);
// blue sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, blue);
ac.InsertChild(s, Matrix4d.CreateTranslation(-0.02, 0.05, 0.05) * Matrix4d.Scale(0.9));
// orange sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, orange);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, -0.02) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Blue-yellow edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "BY", null, new Vector3d(-1.6, -1.6, -0.5)); // BY
ac.InsertChild(c, Matrix4d.Identity);
// blue sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, blue);
ac.InsertChild(s, Matrix4d.CreateTranslation(-0.02, 0.05, 0.05) * Matrix4d.Scale(0.9));
// yellow sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, yellow);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, -0.02, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Red-yellow edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "RY", null, new Vector3d(-0.5, -1.6, 0.6)); // RY
ac.InsertChild(c, Matrix4d.Identity);
// red sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, red);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, 0.12) * Matrix4d.Scale(0.9));
// yellow sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, yellow);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, -0.02, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Green-yellow edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "GY", null, new Vector3d(0.6, -1.6, -0.5)); // GY
ac.InsertChild(c, Matrix4d.Identity);
// yellow sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, yellow);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, -0.02, 0.05) * Matrix4d.Scale(0.9));
// green sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, green);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.12, 0.05, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Orange-yellow edge
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "OY", null, new Vector3d(-0.5, -1.6, -1.6)); // OY
ac.InsertChild(c, Matrix4d.Identity);
// yellow sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, yellow);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, -0.02, 0.05) * Matrix4d.Scale(0.9));
// orange sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, orange);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, -0.02) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#endregion

#region Corners
#region Blue-red-white corner
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "BRW", null, new Vector3d(-1.6, 0.6, 0.6)); // BRW
ac.InsertChild(c, Matrix4d.Identity);
// white sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, white);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.12, 0.05) * Matrix4d.Scale(0.9));
// blue sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, blue);
ac.InsertChild(s, Matrix4d.CreateTranslation(-0.02, 0.05, 0.05) * Matrix4d.Scale(0.9));
// red sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, red);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, 0.12) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Green-red-white corner
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "GRW", null, new Vector3d(0.6, 0.6, 0.6)); // GRW
ac.InsertChild(c, Matrix4d.Identity);
// white sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, white);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.12, 0.05) * Matrix4d.Scale(0.9));
// red sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, red);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, 0.12) * Matrix4d.Scale(0.9));
// green sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, green);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.12, 0.05, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Green-orange-white corner
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "GOW", null, new Vector3d(0.6, 0.6, -1.6)); // GOW
ac.InsertChild(c, Matrix4d.Identity);
// white sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, white);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.12, 0.05) * Matrix4d.Scale(0.9));
// orange sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, orange);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, -0.02) * Matrix4d.Scale(0.9));
// green sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, green);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.12, 0.05, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Blue-orange-white corner
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "BOW", null, new Vector3d(-1.6, 0.6, -1.6)); // BOW
ac.InsertChild(c, Matrix4d.Identity);
// blue sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, blue);
ac.InsertChild(s, Matrix4d.CreateTranslation(-0.02, 0.05, 0.05) * Matrix4d.Scale(0.9));
// orange sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, orange);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, -0.02) * Matrix4d.Scale(0.9));
// white sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, white);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.12, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Blue-red-yellow corner
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "BRY", null, new Vector3d(-1.6, -1.6, 0.6)); // BRY
ac.InsertChild(c, Matrix4d.Identity);
// blue sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, blue);
ac.InsertChild(s, Matrix4d.CreateTranslation(-0.02, 0.05, 0.05) * Matrix4d.Scale(0.9));
// red sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, red);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, 0.12) * Matrix4d.Scale(0.9));
// yellow sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, yellow);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, -0.02, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Green-red-yellow corner
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "GRY", null, new Vector3d(0.6, -1.6, 0.6)); // GRY
ac.InsertChild(c, Matrix4d.Identity);
// red sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, red);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, 0.12) * Matrix4d.Scale(0.9));
// yellow sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, yellow);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, -0.02, 0.05) * Matrix4d.Scale(0.9));
// green sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, green);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.12, 0.05, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Green-orange-yellow corner
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "GOY", null, new Vector3d(0.6, -1.6, -1.6)); // GOY
ac.InsertChild(c, Matrix4d.Identity);
// yellow sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, yellow);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, -0.02, 0.05) * Matrix4d.Scale(0.9));
// orange sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, orange);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, -0.02) * Matrix4d.Scale(0.9));
// green sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, green);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.12, 0.05, 0.05) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#region Blue-orange-yellow
c = new Cube();
ac = new AnimatedNodeTransform(a, null, "BOY", null, new Vector3d(-1.6, -1.6, -1.6)); // BOY
ac.InsertChild(c, Matrix4d.Identity);
// blue sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, blue);
ac.InsertChild(s, Matrix4d.CreateTranslation(-0.02, 0.05, 0.05) * Matrix4d.Scale(0.9));
// yellow sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, yellow);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, -0.02, 0.05) * Matrix4d.Scale(0.9));
// orange sticker
s = new Cube();
s.SetAttribute(PropertyName.MATERIAL, orange);
ac.InsertChild(s, Matrix4d.CreateTranslation(0.05, 0.05, -0.02) * Matrix4d.Scale(0.9));
// finish
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, bm);
#endregion
#endregion

// Center
//c = new Cube();
//ac = new AnimatedNodeTransform(a, null, "R", null, new Vector3d(-0.5, -0.5, -0.5)); // Center
//ac.InsertChild(c, Matrix4d.Identity);
//root.InsertChild(ac, Matrix4d.Identity);
//c.SetAttribute(PropertyName.MATERIAL, bm);


//////////////////////////////////////////////////
// If animator was created in this run of script, load keyframes based on parameters set during scene creation
if (a != null) {
  a.LoadKeyframes();
  context[PropertyName.CTX_START_ANIM] = a.Start;
  context[PropertyName.CTX_END_ANIM] = a.End;
  context[PropertyName.CTX_FPS] = 25.0;
}
