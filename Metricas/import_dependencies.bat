@echo off
echo Creando entorno virtual...

python -m venv venv

echo Activando entorno...
call venv\Scripts\activate

echo Actualizando pip...
python -m pip install --upgrade pip

echo Instalando dependencias...
pip install matplotlib pandas 

echo.
echo Entorno listo. Para activarlo en el futuro usa:
echo venv\Scripts\activate
pause