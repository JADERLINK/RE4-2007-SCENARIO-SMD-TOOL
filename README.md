# RE4-2007-SCENARIO-SMD-TOOL
Extract and repack RE4 2007 scenario smd/pmd files

Translate from Portuguese Brazil

Programas destinados a extrair e recompactar os cenários usando somente um arquivo .OBJ

 ## RE4_2007_SCENARIO_SMD_Extractor.exe

Programa destinado para extrair o cenário, em um arquivo bat use o seguinte comando:
<br>Extract Scenario:
<br>RE4_2007_SCENARIO_SMD_Extractor.exe ".smd File patch" "Pmds Files folder" "Pmd base name"

<br>Exemplo:
<br>RE4_2007_SCENARIO_SMD_Extractor.exe "D:\Games\Re4\st2\r209\r209_04.SMD" "D:\Games\Re4\xscr\r209" "r209"

<br> nesse exemplo será gerado os arquivos na pasta : D:\Games\Re4\xscr\r209

* r209.scenario.idxscenario  // arquivo importante de configurações;
* r209.scenario.obj // conteúdo de todo o cenário, esse é o arquivo que você vai editar;
* r209.scenario.mtl // arquivo que acompanha o .obj;
* r209.scenario.DrawDistance.obj // arquivo informacional, mas não é utilizado no repack.

**Sobre rXXX.scenario.obj**
<br>Esse arquivo é onde esta todo o cenário, nele os arquivos PMD são reparados por grupos, no qual é nomenclatura deve ser respeitada:
<br> Exemplo:
<br> SCENARIO#PMD_000#SMX_001#TYPE_09#
<br> SCENARIO#PMD_001#SMX_002#TYPE_09#

Sendo:
* É obrigatório o nome do grupo começar com "SCENARIO", e ser divido por #
* PMD_000 esse é o ID do arquivo .PMD, a numeração é em decimal
* SMX_001 esse é o ID do SMX, veja o arquivo .SMX,  a numeração é em decimal
* TYPE_09 esse não sei oque é, a numeração é em hexadecimal.
* o nome do grupo deve terminar com # (pois depois de salvo o arquivo, o blender coloca mais texto no final do nome do grupo)

**Editando o arquivo .obj no Blender**
<br>No importador de .obj marque a caixa "Split By Group" que esta no lado direto da tela.
<br>Com o arquivo importado, cada objeto representa um arquivo .PMD
![Groups](Groups.png)
<br>Nota: caso você tenha problema com texturas transparente ficando pretas use esse plugin: (**[link](https://github.com/JADERLINK/Blender_Transparency_Fix_Plugin)**) 

**Ao salvar o arquivo**
<br>Marque as caixas "Triangulated Mesh" e "Object Groups" e "Colors".
<br> no arquivo .obj o nome dos grupos vão ficar com "_Mesh" no final do nome (por isso no editor termina o nome do grupo com # para evitar problemas)

## RE4_2007_SCENARIO_SMD_Repack.exe
Faz o repack do cenário, recebe como parâmetro o arquivo ".idxscenario", 
<br>que no exemplo seria o arquivo "r209.scenario.idxscenario"
<br>e na pasta deve ter os arquivos "r209.scenario.obj" e " r209.scenario.mtl"
<br> e como resultado ira gerar os arquivos:
* r209_000.pmd, r209_001.pmd, r209_002.pmd, r209_003.pmd, ate o ultimo que vai ate r209_126.pmd (nesse exemplo)
* e o arquivo "r209_04.SMD", no qual esse arquivo você deve colocar na pasta "D:\Games\Re4\st2\r209" e recompilar o seu arquivo .DAT


**Sobre rXXX.scenario.idxscenario**
<br>segue a baixo a lista de comando mais importantes presente no arquivo:

* SmdAmount:127 // representa a quantidade de entradas/arquivos Smd/Pmd (você pode mudar mas em alguns cenário pode esta crashando o jogo)
* SmdFileName:r209_04.SMD // esse é o nome do arquivo Smd que será gerado
* PmdBaseName:r209 // esse é o nome base para os arquivos Pmd
* os outros comando que começam com números são auto descritivos (o numero é o ID do Pmd/Smd) (evite mexer nesses valores)

## DrawDistance
caso o seu modelo fique muito longe da posição original, ele pode começa a aparecer e desaparecer, isso é por causa dos valores que definem a que distancia os modelos iram ser vistos.
<br> mude os valores de "xxx_DrawDistanceNegative*" para -3276,7
<br> e os valores de "xxx_DrawDistancePositive*:" para 3276,7
<br> isso vai garantir que o modelo fique visível, porem pode gerar um bug na iluminação.

## bugs
ao mudar os valores originais dos campos "xxx_scale*", "xxx_angle*", "xxx_position*", "xxx_DrawDistanceNegative*" e "xxx_DrawDistancePositive*", pode ocasionar um bug na iluminação do modelo, no qual não sei como resolver.
<br> caso você souber como resolver esse problema favor entrar em contato.

## Código de terceiro:

[ObjLoader by chrisjansson](https://github.com/chrisjansson/ObjLoader):
Encontra-se no RE4_PMD_Repack, código modificado, as modificações podem ser vistas aqui: [link](https://github.com/JADERLINK/ObjLoader).

**At.te: JADERLINK**
<br>2023-10-02
