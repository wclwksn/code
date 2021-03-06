﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using ScriptCoreLib.Library;
using System.Runtime.InteropServices;

using ScriptCoreLibJavaCard.APDUProxyGenerator.Library;
using javacard.framework;

namespace ScriptCoreLibJavaCard.APDUProxyGenerator
{

	class Program
	{


		static void Main(string[] args)
		{

			var TargetAssembly = Assembly.LoadFile(Path.GetFullPath(args[0]));
			var TargetType = TargetAssembly.GetType(args[1], true);
			var TargetFile = args[2];

			using (var p = new ProtectiveFileStream(new FileInfo(TargetFile)))
			using (var s = new StreamWriter(p))
			using (var w = new CodeWriter(s))
			{
				p.NotModified +=
					delegate
					{
						Console.WriteLine("Not modified. " + TargetFile);
					};

				#region content
				s.BaseStream.SetLength(0);

				w.Statement("// The code was generated. Modifications will be lost if regenerated.");

				var source = from k in TargetType.GetMethods()
							 let CLA = k.GetCustomAttributes(typeof(APDUClassAttribute), false).Cast<APDUClassAttribute>().FirstOrDefault()
							 where CLA != null
							 let INS = k.GetCustomAttributes(typeof(APDUInstructionAttribute), false).Cast<APDUInstructionAttribute>().FirstOrDefault()
							 where INS != null
							 group new { k, CLA, INS } by CLA;


				if (TargetFile.EndsWith(".Dispatch.cs"))
				{
					Console.WriteLine("will create dispatcher... " + TargetType.Name);


					#region .Dispatch.cs for card
                    //w.Statement("using ScriptCoreLibJavaCard.javacard.framework;");
                    w.Statement("using " + typeof(Applet).Namespace + ";");

					w.PartialTypeBlock(TargetType, null,
						delegate
						{
							w.Statement("[System.Runtime.CompilerServices.CompilerGeneratedAttribute]");
							w.Block("internal bool Dispatch(APDU e)",
								delegate
								{
									w.Statement("var buffer = e.getBuffer();");
									w.Statement("var CLA = (byte)buffer[ISO7816Constants.OFFSET_CLA];");
									w.Statement("var INS = (byte)buffer[ISO7816Constants.OFFSET_INS];");

									w.Statement("var P1 = (byte)buffer[ISO7816Constants.OFFSET_P1];");
									w.Statement("var P2 = (byte)buffer[ISO7816Constants.OFFSET_P2];");
									w.Statement("var Pi8 = (short)(((P1 & 0xff) << 8) + (P2 & 0xff));");

									
									foreach (var k in source)
									{
										#region CLA
										w.Region("CLA " + k.Key.CLA.ToHexLiteral(),
											delegate
											{
												w.Block("if (CLA == " + k.Key.CLA.ToHexLiteral() + ")",
													delegate
													{
														var Instructions = k.Select((q, i) => new { q.k, q.CLA, INS = q.CLA.AutoAssignInstructions ? (byte)i : q.INS.INS }).OrderBy(ik => ik.INS).ToArray();

														foreach (var i in Instructions.GroupBy(ik => ik.INS))
														{
															#region INS

															w.Block("if (INS == " + i.Key.ToHexLiteral() + ")",
																delegate
																{
																	// default calling convention...
																	// could be extended

																	if (i.Count() == 1)
																	{
																		DispatcherInvoke(w, i.Single().k);

																		w.Statement("return true;");
																	}
																	else
																	{
																		// we shall distinguish instructions via
																		// byte P1, yay

																		foreach (var jj in
																			from j in i
																			let jpp = j.k.GetParameters()
																			where jpp.Length >= 1
																			let j_P1 = jpp[1]
																			where j_P1.ParameterType == typeof(byte)
																			where (j_P1.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault
																			let j_P1_value = (byte)j_P1.DefaultValue
																			orderby j_P1_value
																			select new { P1 = j_P1_value, j.k, j.CLA, j.INS }
																			)
																		{

																			w.Block("if (P1 == " + jj.P1.ToHexLiteral() + ")",
																				delegate
																				{
																					DispatcherInvoke(w, jj.k);
																					w.Statement("return true;");
																				}
																			);

																		}

																		// maybe invoke the one without a default P1?
																		w.Statement("return false;");
																	}


																}
															);

															#endregion
														}


													}
												);
											}
										);
										#endregion

									}

									w.Statement("return false;");
								}
							);
						}
					);
					#endregion
				}
				else
				{
					Console.WriteLine("will create proxy... " + TargetType.Name);

					#region proxy for host
					//w.Statement("using ScriptCoreLib;");
					w.Statement("using System;");

					w.Block("namespace " + TargetType.Namespace,
						delegate
						{
							w.Statement("[System.Runtime.CompilerServices.CompilerGeneratedAttribute]");
							//w.Statement("[Script]");

							w.Block("public partial class " + TargetType.Name + "Proxy",
								delegate
								{
									#region LengthExpected
									w.Statement("public byte LengthExpected;");
									w.Statement("public bool IsLengthExpectedSpecified;");

									w.Statement("public delegate void ByteArrayAction(byte[] e);");
									w.Statement("public ByteArrayAction Tx;");
									w.Statement("public ByteArrayAction Rx;");

									w.Block("protected byte[] InternalTransmit(byte[] data)",
										delegate
										{
											w.Statement("if (Tx != null) Tx(data);");
											w.Statement("var r = this.Transmitter.Transmit(data);");
											w.Statement("if (Rx != null) Rx(r);");
											w.Statement("return r;");
										}
									);

									//w.Statement("[Script]");
									w.Block("class WithLengthExpected_IDisposable : IDisposable",
										delegate
										{
											w.Statement("byte __LengthExpected;");
											w.Statement("bool __IsLengthExpectedSpecified;");
											w.Statement(TargetType.Name + "Proxy context;");

											w.Block("public WithLengthExpected_IDisposable(" + TargetType.Name + "Proxy context, byte value)",
												delegate
												{
													w.Statement("__LengthExpected = context.LengthExpected;");
													w.Statement("__IsLengthExpectedSpecified = context.IsLengthExpectedSpecified;");

													w.Statement("context.LengthExpected = value;");
													w.Statement("context.IsLengthExpectedSpecified = true;");

													w.Statement("this.context = context;");
												}
											);

											w.Block("public void Dispose()",
												delegate
												{
													w.Statement("this.context.LengthExpected = __LengthExpected;");
													w.Statement("this.context.IsLengthExpectedSpecified = __IsLengthExpectedSpecified;");
												}
											);
										}
									);

									w.Block("public IDisposable WithLengthExpected(byte value)",
										delegate
										{
											w.Statement("return new WithLengthExpected_IDisposable(this, value);");
										}
									);
									#endregion

									#region Tokens
									foreach (var Token in source.SelectMany(k => k).SelectMany(k => new[] { k.INS.InputParameterType, k.INS.OutputParameterType }).Where(k => k != null).Distinct())
									{
										#region GetCleanName
										Func<MethodInfo, string> GetCleanName =
											m =>
											{
												var Prefix = Token.Name + "_";

												if (m.Name.StartsWith(Prefix))
													return m.Name.Substring(Prefix.Length);

												return m.Name;
											};
										#endregion

										//w.Statement("[Script]");
										w.Block("public partial class " + Token.Name,
											delegate
											{
												foreach (var enumerator in source.SelectMany(k => k).Where(k => k.INS.InputParameterType == Token && k.INS.OutputParameterType == Token))
												{
													//w.Statement("[Script]");
													w.Block("public partial class " + GetCleanName(enumerator.k) + "Enumerator",
														delegate
														{
															w.Statement("private readonly " + TargetType.Name + "Proxy Host;");

															w.Block("public " + GetCleanName(enumerator.k) + "Enumerator(" + TargetType.Name + "Proxy Host)",
																delegate
																{
																	w.Statement("this.Host = Host;");
																}
															);

															w.Block("public bool MoveNext()",
																delegate
																{
																	w.Statement("this.Current = " + Token.Name + "." + GetCleanName(enumerator.k) + "(this.Host, this.Current);");
																	w.Statement("return Current != null;");
																}
															);

															w.Block("public " + Token.Name + " Current",
																delegate
																{
																	w.Statement("get;");
																	w.Statement("private set;");
																}
															);
														}
													);

													//w.Statement("[Script]");
													w.Block("public partial class " + GetCleanName(enumerator.k) + "Enumerable",
														delegate
														{
															w.Statement("private readonly " + TargetType.Name + "Proxy Host;");

															w.Block("public " + GetCleanName(enumerator.k) + "Enumerable(" + TargetType.Name + "Proxy Host)",
																delegate
																{
																	w.Statement("this.Host = Host;");
																}
															);

															w.Block("public " + GetCleanName(enumerator.k) + "Enumerator GetEnumerator()",
																delegate
																{
																	w.Statement("return new " + GetCleanName(enumerator.k) + "Enumerator(this.Host);");
																}
															);

														}
													);
												}

												w.Statement("public " + TargetType.Name + "Proxy Host { get; set; }");
												w.Statement("public short Token { get; set; }");

												w.Block("public static implicit operator short(" + Token.Name + " e)",
													delegate
													{
														w.Statement("if (e == null) return -1;");
														w.Statement("return e.Token;");
													}
												);

												w.Block("static private " + Token.Name + " FromBytes(" + TargetType.Name + "Proxy host, byte[] data)",
													delegate
													{
														w.Statement("if (data.Length != 4) return null;");
														w.Statement("if (data[2] != 0x90) return null;");
														w.Statement("if (data[3] != 0x00) return null;");

														w.Statement("var P1 = data[0];");
														w.Statement("var P2 = data[1];");
														w.Statement("var Pi8 = (short)(((P1 & 0xff) << 8) + (P2 & 0xff));");

														w.Statement("if (Pi8 < 0) return null;");


														w.Statement("return new " + Token.Name + " { Host = host, Token = Pi8 };");
													}
												);

											}
										);
									}

									#endregion

									//w.Statement("[Script]");
									w.Block("public interface ITransmitter",
										delegate
										{
											w.Statement("byte[] Transmit(params byte[] e);");
										}
									);

									w.Statement("public ITransmitter Transmitter;");



									//w.Statement("public const long ApplicationAID = " + TargetAssembly.GetCustomAttributes(typeof(AIDAttribute), false).Cast<AIDAttribute>().First().Value + "L;");
									//w.Statement("public const long DefaultInstallationSuffix = " + TargetType.GetCustomAttributes(typeof(AIDAttribute), false).Cast<AIDAttribute>().First().Value + "L;");

									Action<object[]> Transmit =
										c =>
										{
											w.Statement("return this.Transmitter.Transmit(" + c.Aggregate("",
												(seed, value) =>
												{
													var x = value is byte ? "0x" + ((byte)value).ToString("x2") :
														value is int ? "0x" + (((int)value) & 0xFF).ToString("x2") :
														value.ToString();

													if (string.IsNullOrEmpty(seed))
														return x;

													return seed + ", " + x;
												}
											) + ");");
										};

									w.Block("public byte[] SelectApplet()",
										delegate
										{
											var c = new AIDAttribute.Info(TargetType).ToSelectApplet().Select(k => (object)k).ToArray();

											Transmit(c);
										}
									);

									var EnumTypesToDefine = from x in source.SelectMany(k => k)
															from y in x.k.GetParameters()
															let yt = y.ParameterType
															where yt.IsEnum && Enum.GetUnderlyingType(yt) == typeof(byte)
															select yt;

									foreach (var EnumType in EnumTypesToDefine.Distinct())
									{
										w.Block("public enum " + EnumType.Name + " : byte",
											delegate
											{
												var Names = Enum.GetNames(EnumType);
												var Values = (byte[])Enum.GetValues(EnumType);

												for (int i = 0; i < Names.Length; i++)
												{
													w.Statement(Names[i] + " = " + Values[i] + ",");
												}
											}
										);
									}

									var ProxyInstructions = source.SelectMany(k => 
											
										k.Select((q, i) => new { q.k, q.CLA, INS = q.CLA.AutoAssignInstructions ? (q.INS.ToINS((byte)i)) : q.INS })
										
									).ToArray();

									foreach (var i in ProxyInstructions)
									{
										var ik = i.k;
										var CLA = i.CLA.CLA;

										ProxyInvoke(w, Transmit, ik, CLA, i.INS);
									}
								}
							);
						}
					);
					#endregion


				}
				#endregion

			}

		}

		private static void ProxyInvoke(CodeWriter w, Action<object[]> Transmit, MethodInfo ik, byte CLA, APDUInstructionAttribute INS)
		{
			var p = ik.GetParameters();

			Func<Type, string> GetCleanName =
				t =>
				{
					var prefix = t.Name + "_";

					if (ik.Name.StartsWith(prefix))
						return ik.Name.Substring(prefix.Length);

					return ik.Name;
				};


			if (INS.InputParameterType != null)
				w.Block("partial class " + INS.InputParameterType.Name,
					delegate
					{
						var CleanName = GetCleanName(INS.InputParameterType);


						if (p.Length == 2 && p[1].ParameterType == typeof(short))
						{
							var Pi8 = p[1];

							if (INS.OutputParameterType != null)
								w.Block("public " + INS.OutputParameterType.Name + " " + CleanName + "(params byte[] data)",
									delegate
									{
										w.Statement("return FromBytes(this.Host, this.Host." + ik.Name + "(this.Token, data));");
									}
								);
							else
							{
								w.Block("public byte[] " + CleanName + "(params byte[] data)",
									delegate
									{
										w.Statement("return this.Host." + ik.Name + "(this.Token, data);");
									}
								);

								// jsc has a nasty bug for nested params method invocation... could cause out of stack error
								w.Block("public byte[] " + CleanName + "()",
									delegate
									{
										w.Statement("var data = new byte[0];");
										w.Statement("return this.Host." + ik.Name + "(this.Token, data);");
									}
								);
							}
						}
					}
				);

			if (INS.OutputParameterType != null)
				w.Block("partial class " + INS.OutputParameterType.Name,
					delegate
					{
						var CleanName = GetCleanName(INS.OutputParameterType);

						if (p.Length == 2 && p[1].ParameterType == typeof(short))
						{
							var Pi8 = p[1];

							if (INS.InputParameterType != null)
								w.Block("static public " + INS.OutputParameterType.Name + " " + CleanName + "(" + ik.DeclaringType.Name + "Proxy host, " + INS.InputParameterType.Name + " token, params byte[] data)",
										delegate
										{
											w.Statement("return " + CleanName + "(host, (short)token, data);");
										}
								);

							w.Block("static public " + INS.OutputParameterType.Name + " " + CleanName + "(" + ik.DeclaringType.Name + "Proxy host, short " + Pi8.Name + ", params byte[] data)",
								delegate
								{
									w.Statement("return FromBytes(host, host." + ik.Name + "(" + Pi8.Name + ", data));");
								}
							);
						}
					}
				);

			if (p.Length == 1)
			{
				w.Statement("[System.Diagnostics.DebuggerNonUserCode]");
				w.Block("public byte[] " + ik.Name + "(params byte[] data)",
					delegate
					{
						w.Statement("var c = new System.IO.MemoryStream();");
						w.Statement("c.WriteByte(" + CLA.ToHexLiteral() + ");");
						w.Statement("c.WriteByte(" + INS.INS.ToHexLiteral() + ");");
						w.Statement("c.WriteByte(" + 0 + ");");
						w.Statement("c.WriteByte(" + 0 + ");");
						w.Statement("c.WriteByte((byte)data.Length);");
						w.Statement("c.Write(data, 0, data.Length);");

						w.Statement("return InternalTransmit(c.ToArray());");
					}
				);
				return;
			}

			if (p.Length == 2 && p[1].ParameterType == typeof(short))
			{
				var Pi8 = p[1];

				w.Statement("[System.Diagnostics.DebuggerNonUserCode]");
				w.Block("public byte[] " + ik.Name + "(short " + Pi8.Name + ", params byte[] data)",
					delegate
					{
						w.Statement("var c = new System.IO.MemoryStream();");
						w.Statement("c.WriteByte(" + CLA.ToHexLiteral() + ");");
						w.Statement("c.WriteByte(" + INS.INS.ToHexLiteral() + ");");
						w.Statement("c.WriteByte(" + "(byte)((" + Pi8.Name + " >> 8) & 0xff)" + ");");
						w.Statement("c.WriteByte(" + "(byte)(" + Pi8.Name + " & 0xff)" + ");");

						w.Block("if (data.Length > 0)",
							delegate
							{
								w.Statement("c.WriteByte((byte)data.Length);");
								w.Statement("c.Write(data, 0, data.Length);");
							}
						);

						w.Block("if (this.IsLengthExpectedSpecified)",
							delegate
							{
								w.Statement("c.WriteByte(this.LengthExpected);");
							}
						);

						w.Statement("return InternalTransmit(c.ToArray());");

					}
				);



				return;
			}

			if (p.Length == 3
				&& (p[1].ParameterType == typeof(byte) || (p[1].ParameterType.IsEnum && Enum.GetUnderlyingType(p[1].ParameterType) == typeof(byte)))
				&& (p[2].ParameterType == typeof(byte) || (p[2].ParameterType.IsEnum && Enum.GetUnderlyingType(p[2].ParameterType) == typeof(byte)))
				)
			{
				var P1 = p[1];
				var P2 = p[2];
				var P1_HasDefault = (P1.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault;
				var P2_HasDefault = (P2.Attributes & ParameterAttributes.HasDefault) == ParameterAttributes.HasDefault;

				var DataParameters = new ParameterInfo[0];

				if (INS.DataParameters != null && INS.DataParameters.BaseType == typeof(MulticastDelegate))
				{
					DataParameters = INS.DataParameters.GetMethod("Invoke").GetParameters();
				}

				#region ParameterString
				var ParameterString = new StringBuilder();

				ParameterString.Append("public byte[] " + ik.Name + "(");

				if (!P1_HasDefault)
				{
					ParameterString.Append(P1.ParameterType.ToPrimitiveName());
					ParameterString.Append(" " + P1.Name + ", ");
				}

				if (!P2_HasDefault)
				{
					ParameterString.Append(P2.ParameterType.ToPrimitiveName());
					ParameterString.Append(" " + P2.Name + ", ");
				}

				if (DataParameters.Length > 0)
				{
					for (int i = 0; i < DataParameters.Length; i++)
					{
						if (i > 0)
							ParameterString.Append(", ");

						ParameterString.Append(DataParameters[i].ParameterType.ToPrimitiveName());
						ParameterString.Append(" ");
						ParameterString.Append(DataParameters[i].Name);
					}
				}
				else
				{
					ParameterString.Append("params byte[] data");
				}

				ParameterString.Append(")");
				#endregion



				w.Statement("[System.Diagnostics.DebuggerNonUserCode]");
				w.Block(ParameterString.ToString(),
					delegate
					{
						#region datac
						if (DataParameters.Length > 0)
							w.Region("data",
								delegate
								{
									w.Statement("var datac = new System.IO.MemoryStream();");

									foreach (var datap in DataParameters)
									{
										if (datap.ParameterType == typeof(string))
										{
											w.Statement("foreach (var datap in " + datap.Name + ") datac.WriteByte((byte)datap);");
										}
									}

									w.Statement("var data = datac.ToArray();");
								}
							);
						#endregion

						w.Statement("var c = new System.IO.MemoryStream();");
						w.Statement("c.WriteByte(" + CLA.ToHexLiteral() + ");");
						w.Statement("c.WriteByte(" + INS.INS.ToHexLiteral() + ");");
						w.Statement("c.WriteByte((byte)" + (P1_HasDefault ? P1.DefaultValue : P1.Name) + ");");
						w.Statement("c.WriteByte((byte)" + (P2_HasDefault ? P2.DefaultValue : P2.Name) + ");");

						w.Block("if (data.Length > 0)",
							delegate
							{
								w.Statement("c.WriteByte((byte)data.Length);");
								w.Statement("c.Write(data, 0, data.Length);");
							}
						);

						w.Block("if (this.IsLengthExpectedSpecified)",
							delegate
							{
								w.Statement("c.WriteByte(this.LengthExpected);");
							}
						);


						w.Statement("return InternalTransmit(c.ToArray());");

					}
				);




				return;
			}
		}

		private static void DispatcherInvoke(CodeWriter w, MethodInfo ik)
		{
			var p = ik.GetParameters();

			if (p.Length == 1)
			{
				w.Statement("this." + ik.Name + "(e);");
				return;
			}

			if (p.Length == 2 && p[1].ParameterType == typeof(short))
			{
				w.Statement("this." + ik.Name + "(e, Pi8);");
				return;
			}



			if (p.Length == 3
				&& (p[1].ParameterType == typeof(byte) || (p[1].ParameterType.IsEnum && Enum.GetUnderlyingType(p[1].ParameterType) == typeof(byte)))
				&& (p[2].ParameterType == typeof(byte) || (p[2].ParameterType.IsEnum && Enum.GetUnderlyingType(p[2].ParameterType) == typeof(byte))))
			{
				w.Statement("this." + ik.Name + "(e, (" + p[1].ParameterType.ToPrimitiveName() + ")P1, (" + p[2].ParameterType.ToPrimitiveName() + ")P2);");
				return;
			}
		}
	}
}
