﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Montador
{
	public class Linha
	{
		public string[] preprocessado;	//a linha depois do preprocessamento
		public int[] tipos;
		public int linhaFonte;	//a linha correspondente no codigo fonte original
		public string[] nomes;	//os nomes dos enderecos,labels e registradores usados, na ordem em que aparecem

		public Linha(string[] preprocessado,int nlinha)
		{
			this.preprocessado = preprocessado;
			this.tipos = new int[preprocessado.Length];
			this.nomes = new string[preprocessado.Length];
			this.linhaFonte = nlinha;
		}
	}
}
