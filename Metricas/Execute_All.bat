@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script stress_metrics...
python src\stress_metrics.py
echo.
echo Ejecucion stress_metrics finalizada.

echo Ejecutando script spatial_heatmap...
python src\spatial_heatmaps.py
echo.
echo Ejecucion spatial_heatmap finalizada.
echo Actualizada/Generada carpeta graphics.

echo Ejecutando script stress_analysis...
python -m nbconvert --to notebook --execute src\stress_analysis.ipynb --inplace
echo.
echo Ejecucion stress_analysis finalizada.
echo Actualizada/Generada carpeta graphics.

pause
