@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script...

python src\stress_metrics.py

echo.
echo Ejecucion finalizada.
pause