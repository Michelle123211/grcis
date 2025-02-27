using System.Collections.Generic;
using System.Linq;
using System.IO;
using DavidSosvald_MichalTopfer;

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
  string keyframes_file = Path.Combine(Path.GetDirectoryName((string)context[PropertyName.CTX_SCRIPT_PATH]), "AnimatorExample.yaml");
  a = new Animator(keyframes_file);
  scene.Animator = a;
  context["animator"] = a;
}

AnimatedCSGInnerNode root = new AnimatedCSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] { 1.0, 0.6, 0.1 }, 0.1, 0.8, 0.2, 16));
scene.Intersectable = root;

// Background color:
scene.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };
scene.Background = new DefaultBackground(scene.BackgroundColor);

// Light sources:
scene.Sources = new LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.8));
scene.Sources.Add(new PointLightSource(new Vector3d(-5.0, 3.0, -3.0), 1.0));

// Camera:
scene.Camera = new KeyframesAnimatedStaticCamera(a);

// --- NODE DEFINITIONS ----------------------------------------------------

// Params dictionary:
Dictionary<string, string> p = Util.ParseKeyValueList(param);

// materials
PhongMaterial pm = new PhongMaterial(new double[] { 1.0, 0.6, 0.1 }, 0.1, 0.8, 0.1, 16);
PhongMaterial r = new PhongMaterial(new double[] { 0.8, 0.1, 0.1 }, 0.1, 0.8, 0.1, 16);
PhongMaterial g = new PhongMaterial(new double[] { 0.1, 1.0, 0.2 }, 0.1, 0.8, 0.1, 16);

// Base plane
Plane pl = new Plane();
pl.SetAttribute(PropertyName.COLOR, new double[] { 0.6, 0.0, 0.0 });
pl.SetAttribute(PropertyName.TEXTURE, new CheckerTexture(0.5, 0.5, new double[] { 1.0, 1.0, 1.0 }));
root.InsertChild(pl, Matrix4d.RotateX(-MathHelper.PiOver2) * Matrix4d.CreateTranslation(0.0, -1.0, 0.0));

// Cubes
Cube c;
AnimatedNodeTransform ac;

// animated cubes
c = new Cube();
ac = new AnimatedNodeTransform(a, "t1", null, "s1");
ac.InsertChild(c, Matrix4d.Identity);
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, r);

c = new Cube();
ac = new AnimatedNodeTransform(a, null, "r1", null, new Vector3d(-5, -0.5, 0));
ac.InsertChild(c, Matrix4d.Identity);
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, g);

c = new Cube();
ac = new AnimatedNodeTransform(a, null, "r1", null, new Vector3d(6, -0.5, 0));
ac.InsertChild(c, Matrix4d.Identity);
root.InsertChild(ac, Matrix4d.Identity);
c.SetAttribute(PropertyName.MATERIAL, g);

// still cubes
// front row - with animated material
AnimatedNodeMaterialColor front = new AnimatedNodeMaterialColor(a);
root.InsertChild(front, Matrix4d.Identity);
PhongMaterial m = new PhongMaterial(new double[] { 0, 0, 0 }, 0.1, 0.8, 0.1, 16);
front.SetAttribute(PropertyName.MATERIAL, m);

// the cubes are copied from the Cubes scene
c = new Cube();
front.InsertChild(c, Matrix4d.RotateY(0.6) * Matrix4d.CreateTranslation(-3.5, -0.8, 0.0));
c = new Cube();
front.InsertChild(c, Matrix4d.RotateY(1.2) * Matrix4d.CreateTranslation(-1.5, -0.8, 0.0));
c = new Cube();
front.InsertChild(c, Matrix4d.RotateY(1.8) * Matrix4d.CreateTranslation(0.5, -0.8, 0.0));
c = new Cube();
front.InsertChild(c, Matrix4d.RotateY(2.4) * Matrix4d.CreateTranslation(2.5, -0.8, 0.0));
c = new Cube();
front.InsertChild(c, Matrix4d.RotateY(3.0) * Matrix4d.CreateTranslation(4.5, -0.8, 0.0));

// back row - not animated
c = new Cube();
root.InsertChild(c, Matrix4d.RotateX(3.5) * Matrix4d.CreateTranslation(-4.0, 1.0, 2.0));
c.SetAttribute(PropertyName.MATERIAL, pm);
c = new Cube();
root.InsertChild(c, Matrix4d.RotateX(3.0) * Matrix4d.CreateTranslation(-2.5, 1.0, 2.0));
c.SetAttribute(PropertyName.MATERIAL, pm);
c = new Cube();
root.InsertChild(c, Matrix4d.RotateX(2.5) * Matrix4d.CreateTranslation(-1.0, 1.0, 2.0));
c.SetAttribute(PropertyName.MATERIAL, pm);
c = new Cube();
root.InsertChild(c, Matrix4d.RotateX(2.0) * Matrix4d.CreateTranslation(0.5, 1.0, 2.0));
c.SetAttribute(PropertyName.MATERIAL, pm);
c = new Cube();
root.InsertChild(c, Matrix4d.RotateX(1.5) * Matrix4d.CreateTranslation(2.0, 1.0, 2.0));
c.SetAttribute(PropertyName.MATERIAL, pm);
c = new Cube();
root.InsertChild(c, Matrix4d.RotateX(1.0) * Matrix4d.CreateTranslation(3.5, 1.0, 2.0));
c.SetAttribute(PropertyName.MATERIAL, pm);
c = new Cube();
root.InsertChild(c, Matrix4d.RotateX(0.5) * Matrix4d.CreateTranslation(5.0, 1.0, 2.0));
c.SetAttribute(PropertyName.MATERIAL, pm);

//////////////////////////////////////////////////
// If animator was created in this run of script, load keyframes based on parameters set during scene creation
if (a != null) {
  a.LoadKeyframes();
  context[PropertyName.CTX_START_ANIM] = a.Start;
  context[PropertyName.CTX_END_ANIM] = a.End;
  context[PropertyName.CTX_FPS] = 25.0;
}
