@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script...

python -m nbconvert --to notebook --execute src\stress_analysis.ipynb --inplace

echo.
echo Ejecucion finalizada.
echo Actualizada/Generada carpeta graphics.
pause