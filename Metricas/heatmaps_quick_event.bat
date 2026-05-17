@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script...

python src\spatial_heatmaps.py

echo.
echo Ejecucion finalizada.
pause