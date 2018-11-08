import csv
def converter(tiempo):
    tiempo = tiempo.split(":")
    horas, minutos, segundos = int(tiempo[0]), int(tiempo[1]), int(tiempo[2])
    return horas * 60 * 60 + minutos * 60 + segundos

def desconverter(segundos):
    horas = segundos // 3600
    minutos = (segundos - horas * 3600) // 60
    segundos  = segundos - horas * 3600 - minutos * 60
    return str(horas) + ":" + str(minutos) + ":" + str(segundos)
archivo = input("Ingrese nombre del archivo (sin el .txt): ")
file = open(archivo + ".txt")
output = open("Tablas " + archivo + ".csv", "w")
register = []
register1 = []
linea = file.readline()
t0, t1, t2 = 0, 0, 0
t00, t01, t02 = 0, 0, 0
t000, t001, t002 = 0, 0, 0
scene = 0
muertes = 0
mana0, mana1, mana2 = 0, 0, 0
inac0, inac1, inac2 = 0, 0, 0
pois0, pois1, pois2 = [], [], []
exps = []
while linea:
    while ":" not in linea and linea:
        linea = file.readline()
    if not linea:
        break
    if "Player 0" in linea or "Player0" in linea:
        if converter(linea.split(" ")[0]) - t0 > 10 and t0 != 0:
            register.append(["Más de 10 segundos de inactividad Player0 entre " + desconverter(t0)
                             + " y " + desconverter(converter(linea.split(" ")[0]))])
            inac0 += 1
        t0 = converter(linea.split(" ")[0])
    elif "Player 1" in linea or "Player1" in linea:
        if converter(linea.split(" ")[0]) - t1 > 10 and t1 != 0:
            register.append(["Más de 10 segundos de inactividad Player1 entre " + desconverter(t1)
                             + " y " + desconverter(converter(linea.split(" ")[0]))])
            inac1 += 1
        t1 = converter(linea.split(" ")[0])
    elif "Player 2" in linea or "Player2" in linea:
        if converter(linea.split(" ")[0]) - t2 > 10 and t2 != 0:
            register.append(["Más de 10 segundos de inactividad Player2 entre " + desconverter(t2)
                             + " y " + desconverter(converter(linea.split(" ")[0]))])
            inac2 += 1
        t2 = converter(linea.split(" ")[0])
    if "Poi number: 1" in linea:
        if linea.split(" ")[-1].strip() == "Player0":
            t_inicio_etapa0 = converter(linea.split(" ")[0])
            t_puzzle_anterior0 = t_inicio_etapa0
            register.append(["Empezo la etapa para Player0 a las " + desconverter(t_inicio_etapa0)])
        if linea.split(" ")[-1].strip() == "Player1":
            t_inicio_etapa1 = converter(linea.split(" ")[0])
            t_puzzle_anterior1 = t_inicio_etapa1
            register.append(["Empezo la etapa para Player1 a las " + desconverter(t_inicio_etapa0)])
        if linea.split(" ")[-1].strip() == "Player2":
            t_inicio_etapa2 = converter(linea.split(" ")[0])
            t_puzzle_anterior2 = t_inicio_etapa2
            register.append(["Empezo la etapa para Player2 a las " + desconverter(t_inicio_etapa0)])
    elif "Poi number:" in linea:
        if linea.split(" ")[-1].strip() == "Player0":
            number = linea.split(" ")[3]
            tiempo = converter(linea.split(" ")[0]) - t_puzzle_anterior0
            t_puzzle_anterior0 = converter(linea.split(" ")[0])
            register.append(["player0",number, tiempo])
        if linea.split(" ")[-1].strip() == "Player1":
            number = linea.split(" ")[3]
            tiempo = converter(linea.split(" ")[0]) - t_puzzle_anterior1
            t_puzzle_anterior1 = converter(linea.split(" ")[0])
            register.append(["player1",number, tiempo])
        if linea.split(" ")[-1].strip() == "Player2":
            number = linea.split(" ")[3]
            tiempo = converter(linea.split(" ")[0]) - t_puzzle_anterior2
            t_puzzle_anterior2 = converter(linea.split(" ")[0])
            register.append(["player2",number, tiempo])


    elif " Player 0 entered poi with ID:" in linea:
        tiempo = desconverter(converter(linea.split(" Player 0 entered poi with ID: ")[0]) - t000)
        poi = linea.split(" Player 0 entered poi with ID: ")[1]
        pois0.append([poi,tiempo])
    elif "Player 1 entered poi with ID:" in linea:
        tiempo = desconverter(converter(linea.split(" Player 1 entered poi with ID: ")[0]) - t001)
        poi = linea.split(" Player 1 entered poi with ID: ")[1]
        pois1.append([poi, tiempo])
    elif "Player 2 entered poi with ID:" in linea:
        tiempo = desconverter(converter(linea.split(" Player 2 entered poi with ID: ")[0]) - t002)
        poi = linea.split(" Player 2 entered poi with ID: ")[1]
        pois2.append([poi, tiempo])

    elif "Stoped Charging" in linea:
        if linea.split(" ")[2].strip() == "0":
            if converter(linea.split(" ")[0]) - t00 > 10:
                register.append(["Player 0 estuvo más de 10 segundos recargando Maná entre " + desconverter(t00) + " y "
                                + linea.split(" ")[0]])
                mana0 += 1
        elif linea.split(" ")[2].strip() == "1":
            if converter(linea.split(" ")[0]) - t01 > 10:
                register.append(["Player 1 estuvo más de 10 segundos recargando Maná entre " + desconverter(t00) + " y "
                                + linea.split(" ")[0]])
                mana1 += 1
        elif linea.split(" ")[2].strip() == "2":
            if converter(linea.split(" ")[0]) - t02 > 10:
                register.append(["Player 2 estuvo más de 10 segundos recargando Maná entre " + desconverter(t00) + " y "
                                + linea.split(" ")[0]])
                mana2 += 1

    elif "Charging" in linea:
        if linea.split(" ")[2].strip() == "0":
            t00 = converter(linea.split(" ")[0])
        elif linea.split(" ")[2].strip() == "1":
            t01 = converter(linea.split(" ")[0])
        elif linea.split(" ")[2].strip() == "2":
            t02 = converter(linea.split(" ")[0])

    # equipo
    if "was reached by all the necessary" in linea:
        tiempo, poi = linea.strip().split(" ")[-1], linea.split(" ")[3]
        register1.append([scene, float(tiempo), "poi numero: " + poi + " completado en " + tiempo])
    if "CHANGING SCENE!  The players gathered" in linea:
        exp = linea.split(" ")[-3]
        register1.append([0,"se recolectaron " + exp + "puntos de experiencia en la escena " + str(scene)])
        exps.append(exp)
        scene += 1
    if "PLAYERS ARE DEAD!!" in linea:
        muertes += 1

    linea = file.readline()


# for i in register: print(i)
# print("\n\n")
# print(sorted(register1, reverse=True)[0])
# register1.sort(reverse=True)
# print("\n\n")
# for i in register1: print(i)
# print("etapas superadas:", str(scene))
# print("cantidad de muertes:", str(muertes))

output.write("Individual\n \n")
output.write("Players,0,1,2\n")
output.write("Mana," + str(mana0) + "," + str(mana1) + "," + str(mana2) + "\n")
output.write("Inactividad," + str(inac0) + "," + str(inac1) + "," + str(inac2) + "\n")
for i in range(min([len(pois0), len(pois1), len(pois2)])):
    output.write("POI-" + pois0[i][0] + "," + pois0[i][1] + "," + pois1[i][1] + "," + pois2[i][1] + "\n")
output.write(" \nGrupal General\n \n")
output.write("Etapas Superadas," + str(scene) + "\n")
output.write("Cantidad de Muertes," + str(muertes) + "\n \n")
output.write("Grupal por Escena\n \n")
escenas = ""
for i in range(scene + 1):
    escenas += str(i) + ","
escenas.strip(",")
output.write("Escena," + escenas + "\n")
aux = -1
out = "POI mas lento,"
for i in register1:
    if i[0] != aux:
        aux = i[0]
        out += i[2].split(" ")[2] + ","
output.write(out.strip(",") + "\n")
out = "Experiencia,"
for i in exps:
    out += i + ","
output.write(out.strip(",") + "\n")

output.write(" \nTiempo entre POIs por escena\n \n")
aux = -1
out = ""
contador = 0
prev = 0
for i in register1:
    if aux != i[0]:
        out += "\n" + str(i[0]) + ","
        aux = i[0]
        contador = 0
    contador += 1
    if contador > prev:
        prev = contador
    out += str(i[1]) + " secs,"
output.write("Escena/POI,")
for i in range(prev):
    output.write(str(i) + ",")
output.write("\n")
output.write(out)
print("Archivo creado con éxito!")
output.close()